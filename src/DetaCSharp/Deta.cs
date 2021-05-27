using DetaCSharp.Drive;
using DetaCSharp.Utils;
using System;
using System.Net.Http;

namespace DetaCSharp
{
    public class Deta
    {
        private readonly DetaOptions options;
        private readonly HttpClient http;

        public Deta(string projectKey, HttpClient httpClient = null)
        {
            options = DetaOptions.Default(projectKey);
            http = httpClient ?? new HttpClient();
        }

        public DriveClass Drive(string driveName)
        {
            var timmedName = driveName?.Trim();

            if (string.IsNullOrWhiteSpace(timmedName))
            {
                throw new ArgumentException("Drive name is not defined", nameof(driveName));
            }

            return new DriveClass(options, driveName, http);
        }
    }
}
