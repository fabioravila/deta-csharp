using System.Text.Json.Serialization;

namespace DetaCSharp.Drive
{
    public class UploadResponse
    {
        [JsonPropertyName("upload_id")]
        public string UploadId { get; set; }

        public string Name { get; set; }

        public int? Part { get; set; }

        [JsonPropertyName("project_id")]
        public string ProjectId { get; set; }

        [JsonPropertyName("drive_name")]
        public string DriveName { get; set; }

    }
}
