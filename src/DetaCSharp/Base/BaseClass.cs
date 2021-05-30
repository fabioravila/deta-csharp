using DetaCSharp.Constants;
using DetaCSharp.Types;
using DetaCSharp.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DetaCSharp.Base
{
    public class BaseClass
    {
        private string baseHostUrl;
        private RequestsHelper requestHelper;
        private BaseUtils util;

        public BaseClass(DetaOptions options, string baseName, HttpClient httpClient)
        {
            baseHostUrl = options.BaseHostUrl.Replace(":base_name", baseName);
            requestHelper = new RequestsHelper(options.ProjectKey, baseHostUrl, httpClient);
            util = new BaseUtils();
        }

        public async Task<IEnumerable<ItemKeyResponse>> Put(object data, string key = null)
        {
            var item = CreateItemPayload(data, key);

            var response = await requestHelper.Put<PutResponse>(BaseApi.PUT_ITEMS, new
            {
                items = new[] { item }
            });


            response.EnsureSuccess();


            //TODO: For now we just return a key as string
            return response.Body?.Processed?.Items ?? null;
        }

        public async Task<T> Get<T>(string key)
        {
            var trimmedKey = key?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedKey))
            {
                throw new ArgumentException("Key is empty", nameof(key));
            }

            var encodedKey = HttpUtility.UrlEncode(trimmedKey);

            var response = await requestHelper.Get<T>(BaseApi.GET_ITEMS.Replace(":key", encodedKey));

            if (response.IsError && response.Status != HttpStatusCode.NotFound)
            {
                throw response.Error;
            }


            if (response.Status == HttpStatusCode.OK)
            {
                return response.Body;
            }


            //404 respose for sure
            return default;
        }

        public async Task Delete(string key)
        {
            var trimmedKey = key?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedKey))
            {
                throw new ArgumentException("Key is empty", nameof(key));
            }

            var encodedKey = HttpUtility.UrlEncode(trimmedKey);


            var response = await requestHelper.Delete(BaseApi.DELETE_ITEMS.Replace(":key", encodedKey));

            response.EnsureSuccess();
        }

        public async Task<string> Insert(object data, string key = null)
        {
            var itemPayload = CreateItemPayload(data, key);

            var response = await requestHelper.Post<ItemKeyResponse>(BaseApi.INSERT_ITEMS, new RequestInit
            {
                Payload = new
                {
                    item = itemPayload
                }
            });

            if (response.IsError && response.Status == HttpStatusCode.Conflict)
            {
                throw new DetaException(response.Status, $"Item with key {key} already exists");
            }


            response.EnsureSuccess();

            return response.Body?.Key ?? null;
        }

        public async Task<PutResponse> PutMany(IEnumerable<object> items)
        {
            if (!items.Any())
            {
                throw new ArgumentException("Items can't be empty", nameof(items));
            }

            if (items.Count() > 25)
            {
                throw new ArgumentException("We can't put more than 25 items at a time");
            }


            var itemsPayload = items.Select(i => CreateItemPayload(i, key: null))
                                    .ToArray();

            var response = await requestHelper.Put<PutResponse>(BaseApi.PUT_ITEMS, new
            {
                items = itemsPayload
            });


            response.EnsureSuccess();


            return response.Body;
        }



        public async Task<ItemKeyResponse> Update(object updates, string key)
        {
            var trimmedKey = key?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedKey))
            {
                throw new ArgumentException("Key is empty", nameof(key));
            }

            object payload = null;
            if (updates is Update)
            {
                payload = updates;
            }
            else
            {

                //TODO: This is a hacky too, come back here later
                var updatePayload = CreateUpdate();

                foreach ((var objKey, var value) in ToDictionary(updates))
                {
                    var action = value is DetaAction ? (DetaAction)value : new DetaAction(ActionTypes.Set, value);

                    updatePayload.With(action.Action, objKey, value);
                }

                payload = updatePayload;
            }

            var encodedKey = HttpUtility.UrlEncode(trimmedKey);
            var response = await requestHelper.Patch<PutResponse>(BaseApi.PATCH_ITEMS.Replace(":key", encodedKey), payload);

            response.EnsureSuccess();

            return new ItemKeyResponse
            {
                Key = trimmedKey
            };
        }

        public async IAsyncEnumerable<T> Fetch<T>(object query = null, int pages = 10, int? buffer = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var lastValue = "";

            object q = null;
            if (query is Query)
            {
                q = query;
            }
            else
            {
                q = IsArray(query) ? (object[])query : new[] { query };
            }


            for (int i = 0; i < pages; i++)
            {
                var response = await requestHelper.Post<QueryResponse<T>>(BaseApi.QUERY_ITEMS, new RequestInit
                {
                    Payload = new
                    {
                        query = q,
                        limit = buffer,
                        last = lastValue
                    }
                });

                response.EnsureSuccess();

                lastValue = response.Body.Paging.Last;

                foreach (var item in response.Body.Items)
                {
                    yield return item;
                }

                if (string.IsNullOrWhiteSpace(lastValue))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Createa sa Update object sugar to be use in the UpdateMethod
        /// </summary>
        /// <returns></returns>
        public Update CreateUpdate() => new Update(requestHelper.JsonSerializerOptions.PropertyNamingPolicy);

        /// <summary>
        /// Creates a query object sugar to use i Query methods
        /// </summary>
        /// <returns></returns>
        public Query CreateQuery() => new Query(requestHelper.JsonSerializerOptions.PropertyNamingPolicy);

        //Check is a type is consideres simple by deta (array, string, number, boolean)
        static bool IsDetaSimpleType(object data)
        {
            var type = data.GetType();

            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || data is IEnumerable;
        }

        static bool IsArray(object data) => data is IEnumerable;

        IDictionary<string, object> ToDictionary(object data)
        {
            var dictionary = new JsonNamingPolicyDictionary(requestHelper.JsonSerializerOptions.PropertyNamingPolicy);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(data))
            {
                dictionary.Add(property.Name, property.GetValue(data));
            }

            return dictionary;
        }

        public static IDictionary<string, object> ToDictionaryWithSerialize(object data)
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(data));
        }

        object CreateItemPayload(object data, string key)
        {
            //TODO: THis is very hacky as i dont know about performance issue, so back here later to optmize
            if (IsDetaSimpleType(data))
            {
                return string.IsNullOrWhiteSpace(key) ? (object)new { value = data } : (object)new { key = key, value = data };
            }


            if (string.IsNullOrWhiteSpace(key))
            {
                return data;
            }

            //preference is for key properties
            var dict = ToDictionary(data);
            dict["key"] = key;

            return dict;
        }
    }
}
