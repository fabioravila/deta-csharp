using DetaCSharp.Constants;
using DetaCSharp.Types;
using DetaCSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DetaCSharp.Drive
{
    public class DriveClass
    {
        private readonly DetaOptions options;
        private readonly string diveHostUrl;
        private readonly RequestsHelper requestHelper;

        public DriveClass(DetaOptions options, string driveName, HttpClient httpClient)
        {
            diveHostUrl = options.DriveHostUrl.Replace(":drive_name", driveName);
            requestHelper = new RequestsHelper(options.ProjectKey, diveHostUrl, httpClient);
        }

        public async Task<Stream> Get(string name)
        {
            var trimmedName = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Name is empty", nameof(name));
            }


            var encodedName = HttpUtility.UrlEncode(trimmedName);

            var response = await requestHelper.Get<Unit>(DriveApi.GET_FILE.Replace(":name", encodedName));

            if (response.IsError && response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccess();


            return response.BodyStream;
        }

        public async Task Delete(string name)
        {
            var trimmedName = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Name is empty", nameof(name));
            }


            var response = await requestHelper.Delete(DriveApi.DELETE_FILES, new
            {
                names = new[] { trimmedName }
            });


            response.EnsureSuccess();

            //TODO : Adjus helper fo serialize correct response

        }

        public async Task DeleteMany(IEnumerable<string> names)
        {
            if (!names.Any())
            {
                throw new ArgumentException("Names can't be empty", nameof(names));
            }

            if (names.Count() > 1000)
            {
                throw new ArgumentException("We can't delete more than 1000 items at a time", nameof(names));
            }



            var response = await requestHelper.Delete(DriveApi.DELETE_FILES, new
            {
                names = names
            });


            if (response.IsError && response.Status == System.Net.HttpStatusCode.BadRequest)
            {
                throw new DetaException(response.Status, "Names can't be empty");
            }

            response.EnsureSuccess();

            //TODO : Adjust Helper for serialize correct response

        }

        public async Task<ListResponse> List(ListOptions options = null)
        {
            var opt = options ?? ListOptions.Empty;

            var response = await requestHelper.Get<ListResponse>(DriveApi.LIST_FILES.Replace(":prefix", opt.Prefix ?? "")
                                                                                    .Replace(":limit", (opt.Limit ?? 1000).ToString())
                                                                                    .Replace(":last", opt.Last ?? ""));

            response.EnsureSuccess();


            return response.Body;
        }

        public async Task<string> Put(string name, PutOptions options)
        {
            var trimmedName = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Name is empty", nameof(name));
            }

            var encodedName = HttpUtility.UrlEncode(trimmedName);

            if (!string.IsNullOrWhiteSpace(options.Path) && options.Data != null)
            {
                throw new ArgumentException("Please only provide data or a path. Not both");
            }

            if (string.IsNullOrWhiteSpace(options.Path) && options.Data is null)
            {
                throw new ArgumentException("Please provide data or a path. Both are empty");
            }

            var streamData = ExtractStreamData(options, out bool ownStream);

            try
            {
                var response = await Upload(encodedName, streamData, options.ContentType ?? "binary/octet-stream");

                if (response.Error != null)
                {
                    throw response.Error;
                }

                return response.Response as string;
            }
            finally
            {
                if (ownStream && streamData != null)
                {
                    streamData.Close();
                    streamData.Dispose();
                }
            }
        }

        static Stream ExtractStreamData(PutOptions options, out bool ownStream)
        {
            Stream streamData = null;
            ownStream = false;

            if (!string.IsNullOrWhiteSpace(options.Path))
            {
                streamData = new FileStream(options.Path, FileMode.Open, FileAccess.Read);
                ownStream = true;
            }

            if (options.Data != null)
            {
                if (options.Data is Stream)
                {
                    streamData = options.Data as Stream;
                }
                else if (options.Data is string)
                {
                    streamData = new MemoryStream(Encoding.UTF8.GetBytes(options.Data as string));
                    ownStream = true;
                }
                else if (options.Data is byte[])
                {
                    streamData = new MemoryStream(options.Data as byte[]);
                    ownStream = true;
                }
                else
                {
                    throw new ArgumentException("Unsupported data format, expected data to be one of: string | byte[] | Stream");
                }
            }

            return streamData;
        }

        async Task<UploadResponse> Upload(string name, Stream streamData, string contentType)
        {
            //TODO: Create a algo to read the stream in chunks, so no ned to put a memorystream here
            //NOTE: Putt all code in one method to avoid context in action
            var chunkSize = 1024 * 1024 * 10; //10MB

            var requestInit = new RequestInit
            {
                Headers = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Content-Type", contentType)
                }
            };

            //INT_FILE
            var response = await requestHelper.Post<UploadApiResponse>(DriveApi.INIT_CHUNK_UPLOAD.Replace(":name", name), requestInit);
            response.EnsureSuccess();

            var uploadId = response.Body.UploadId;


            //CHUNK_FILE
            var part = 1;
            var buffer = new byte[chunkSize];
            int read;
            do
            {
                read = streamData.Read(buffer, 0, buffer.Length);
                if (read > 0)
                {
                    //Need check read < buffer.Lenght?
                    if (read < buffer.Length)
                    {
                        var last = new byte[read];
                        Array.Copy(buffer, last, last.Length);
                        buffer = last;
                    }

                    requestInit.Payload = buffer;

                    response = await requestHelper.Post<UploadApiResponse>(DriveApi.UPLOAD_FILE_CHUNK.Replace(":uid", uploadId)
                                                                                  .Replace(":name", name)
                                                                                  .Replace(":part", part.ToString()), requestInit);

                    if (response.IsError)
                    {
                        return new UploadResponse { Error = response.Error };
                    }


                    part++;
                }
            } while (read > 0);



            //COMPLETE_FILE
            var complete = await requestHelper.Patch(DriveApi.COMPLETE_FILE_UPLOAD.Replace(":uid", uploadId)
                                                                              .Replace(":name", name));

            if (complete.IsError)
            {
                return new UploadResponse { Error = complete.Error };
            }


            return new UploadResponse
            {
                Response = name
            };
        }
    }
}
