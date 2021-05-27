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

            var response = await requestHelper.Get(DriveApi.GET_FILE.Replace(":name", encodedName));

            if (response.IsError && response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (response.IsError)
            {
                throw response.Error;
            }


            return response.Body as Stream;
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


            if (response.IsError)
            {
                throw response.Error;
            }

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

            if (response.IsError)
            {
                throw response.Error;
            }

            //TODO : Adjust Helper for serialize correct response

        }


        public async Task<ListResponse> List(ListOptions options)
        {
            var opt = options ?? new ListOptions();

            var response = await requestHelper.Get(DriveApi.LIST_FILES.Replace(":prefix", opt.Prefix ?? "")
                                                                      .Replace(":limit", (opt.Limit ?? 1000).ToString())
                                                                      .Replace(":last", opt.Last ?? ""));


            if (response.IsError)
            {
                throw response.Error;
            }


            return response.Body as ListResponse;
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


            object data = null;
            if (!string.IsNullOrWhiteSpace(options.Path))
            {
                data = new FileStream(options.Path, FileMode.Open, FileAccess.Read);
            }


            if (options.Data != null)
            {
                if (options.Data is Stream)
                {
                    data = options.Data as Stream;
                }
                else if (options.Data is string)
                {
                    data = Encoding.UTF8.GetBytes(options.Data as string);
                }
                else if (options.Data is byte[])
                {
                    data = options.Data as byte[];
                }
                else
                {
                    throw new ArgumentException("Unsupported data format, expected data to be one of: string | byte[] | Stream");
                }
            }

            if (data is null)
            {
                throw new ArgumentException("Data can´t be null");
            }

            //PS: Only pass Stream or byte[]
            var response = await Upload(encodedName, data, options.ContentType ?? "binary/octet-stream");


            if (response.Error != null)
            {
                throw response.Error;
            }


            return response.Response as string;
        }

        async Task<UploadResponse> Upload(string name, object data, string contentType)
        {
            //TODO: Create a algo to read the stream in chunks, so no ned to put a memorystream here
            var chunkSize = 1024 * 1024 * 10; //10MB

            var requestInit = new RequestInit
            {
                Headers = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Content-Type", contentType)
                }
            };

            //INT_FILE
            var response = await requestHelper.Post(DriveApi.INIT_CHUNK_UPLOAD.Replace(":name", name), requestInit);
            if (response.IsError)
            {
                throw response.Error;
            }


            var uploadId = "upload id get from typed reponse";


            //CHUNK_FILE
            Stream streamData = null;
            var ownStream = false;
            try
            {

                if (data is Stream)
                {
                    streamData = data as Stream;
                }
                else
                {
                    streamData = new MemoryStream(data as byte[]);
                    ownStream = true;
                }

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

                        response = await requestHelper.Post(DriveApi.UPLOAD_FILE_CHUNK.Replace(":uid", uploadId)
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
                response = await requestHelper.Patch(DriveApi.COMPLETE_FILE_UPLOAD.Replace(":uid", uploadId)
                                                                                  .Replace(":name", name));



                if (response.IsError)
                {
                    return new UploadResponse { Error = response.Error };
                }




                return new UploadResponse
                {
                    Response = name
                };
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
    }
}
