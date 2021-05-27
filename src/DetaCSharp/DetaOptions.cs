namespace DetaCSharp
{
    public class DetaOptions
    {
        const string BASE_HOST_URL = "https://database.deta.sh/v1/:project_id/:base_name";
        const string DRIVE_HOST_URL = "https://drive.deta.sh/v1/:project_id/:drive_name";

        public string ProjectKey { get; private set; }
        public string DriveHostUrl { get; private set; }
        public string BaseHostUrl { get; private set; }

        public static DetaOptions Default(string projectKey) => new DetaOptions
        {
            ProjectKey = projectKey,
            DriveHostUrl = DRIVE_HOST_URL,
            BaseHostUrl = BASE_HOST_URL
        };
    }
}