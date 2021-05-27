using DetaCSharp.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DetaCSharp.Utils
{
    public class RequestsHelper
    {
        readonly string projectKey;
        readonly string baseUrl;
        readonly HttpClient http;
        const string JsonContentType = "application/json";


        static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RequestsHelper(string projectKey, string baseUrl, HttpClient http = null)
        {
            var projectId = projectKey.Split("_")[0];
            this.projectKey = projectKey;
            this.baseUrl = baseUrl.Replace(":project_id", projectId);
            this.http = http;
        }


        public Task<Response<Unit>> Put(string uri, object payload) => SendAsync(HttpMethod.Put, uri, payload);
        public Task<Response<T>> Delete<T>(string uri, object payload = null) => SendAsync<T>(HttpMethod.Delete, uri, payload);
        public Task<Response<T>> Get<T>(string uri) => SendAsync<T>(HttpMethod.Get, uri, null);
        public Task<Response<Unit>> Post(string uri, object payload) => SendAsync(HttpMethod.Post, uri, payload);
        public Task<Response<T>> Post<T>(string uri, RequestInit init) => SendAsync<T>(HttpMethod.Post, uri, init.Payload, init.Headers);
        public Task<Response<Unit>> Patch(string uri, object payload = null) => SendAsync(HttpMethod.Patch, uri, payload);

        public Task<Response<T>> Patch<T>(string uri, object payload = null) => SendAsync<T>(HttpMethod.Patch, uri, payload);


        Task<Response<Unit>> SendAsync(HttpMethod method, string path, object payload, IEnumerable<KeyValuePair<string, string>> aditionalHeaders = null)
            => SendAsync<Unit>(method, path, payload, aditionalHeaders);

        async Task<Response<T>> SendAsync<T>(HttpMethod method, string path, object payload, IEnumerable<KeyValuePair<string, string>> aditionalHeaders = null)
        {
            using var request = new HttpRequestMessage(method, new Uri($"{baseUrl}{path}"));

            request.Headers.TryAddWithoutValidation("X-API-Key", projectKey);
            if (aditionalHeaders != null)
            {
                foreach (var header in aditionalHeaders)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }



            var content = GenerateContent(payload);
            if (content != null)
            {
                request.Content = content;
            }


            var response = await http.SendAsync(request);


            //For now, to determine is content is JSON i will use header content
            var streamResponse = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = await DeserializeJsonFromStream<ErrorResponse>(streamResponse);

                var message = error?.Errors?.Length > 0 ? error.Errors[0] : "Something went wrong";

                return new Response<T>
                {
                    Error = new DetaException(response.StatusCode, message),
                    Status = response.StatusCode
                };
            }


            if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType != null &&
                response.Content.Headers.ContentType.MediaType.StartsWith(JsonContentType))
            {
                return new Response<T>
                {
                    Body = await DeserializeJsonFromStream<T>(streamResponse),
                    Status = response.StatusCode
                };
            }
            else
            {
                return new Response<T>
                {
                    Body = default,
                    BodyStream = streamResponse,
                    Status = response.StatusCode
                };
            }
        }

        HttpContent GenerateContent(object payload)
        {
            if (payload is null)
            {
                return null;
            }


            if (payload is byte[])
            {
                return new ByteArrayContent(payload as byte[]);
            }
            else if (payload is Stream)
            {
                return new StreamContent(payload as Stream);
            }
            else if (payload is string)
            {
                return new StringContent(payload as string);
            }
            else
            {
                return new StringContent(JsonSerializer.Serialize(payload, JsonSerializerOptions), Encoding.UTF8, JsonContentType);
            }

        }

        protected async Task<T> DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream is null || stream.CanRead is false)
            {
                return default;
            }

            return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions);
        }


        class ErrorResponse
        {
            public string[] Errors { get; set; }
        }
    }
}
