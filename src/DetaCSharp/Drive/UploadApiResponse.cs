using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DetaCSharp.Drive
{
    public class UploadApiResponse
    {
        [JsonPropertyName("upload_id")]
        public string UploadId { get; set; }
    }
}
