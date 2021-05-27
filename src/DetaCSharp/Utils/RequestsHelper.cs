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

        public RequestsHelper(string projectKey, string baseUrl, HttpClient http = null)
        {
            var projectId = projectKey.Split("_")[0];
            this.projectKey = projectKey;
            this.baseUrl = baseUrl.Replace(":project_id", projectId);
            this.http = http;
        }


        public Task<Response> Put(string uri, object payload) => SendAsync(HttpMethod.Put, uri, payload);
        public Task<Response> Delete(string uri, object payload = null) => SendAsync(HttpMethod.Delete, uri, payload);
        public Task<Response> Get(string uri) => SendAsync(HttpMethod.Get, uri, null);
        public Task<Response> Post(string uri, object payload) => SendAsync(HttpMethod.Post, uri, payload);
        public Task<Response> Post(string uri, RequestInit init) => SendAsync(HttpMethod.Post, uri, init.Payload, init.Headers);
        public Task<Response> Patch(string uri, object payload = null) => SendAsync(HttpMethod.Patch, uri, payload);

        private async Task<Response> SendAsync(HttpMethod method, string path, object payload, IEnumerable<KeyValuePair<string, string>> aditionalHeaders = null)
        {
            using var request = new HttpRequestMessage(method, new Uri($"{baseUrl}${path}"));

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

                return new Response
                {
                    Error = new DetaException(response.StatusCode, message),
                    Status = response.StatusCode
                };
            }


            if (response.Content.Headers.ContentType.MediaType.StartsWith(JsonContentType))
            {
                return new Response
                {
                    Body = DeserializeJsonFromStream<object>(streamResponse),
                    Status = response.StatusCode
                };
            }
            else
            {
                return new Response
                {
                    Body = streamResponse,
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
                return new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, JsonContentType);
            }

        }

        protected async Task<T> DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream is null || stream.CanRead is false)
            {
                return default;
            }

            return await JsonSerializer.DeserializeAsync<T>(stream);
        }


        class ErrorResponse
        {
            public string[] Errors { get; set; }
        }
    }
}
