using DetaCSharp.Base;
using DetaCSharp.Drive;
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
            var trimmedName = driveName?.Trim();

            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Drive name is not defined", nameof(driveName));
            }

            return new DriveClass(options, driveName, http);
        }


        public BaseClass Base(string baseName)
        {
            var trimmedName = baseName?.Trim();

            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Base name is not defined", nameof(baseName));
            }

            return new BaseClass(options, baseName, http);
        }
    }
}
