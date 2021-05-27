
namespace DetaCSharp.Constants
{
    public static class DriveApi
    {
        public const string GET_FILE = "/files/download?name=:name";
        public const string DELETE_FILES = "/files";
        public const string LIST_FILES = "/files?prefix=:prefix&limit=:limit&last=:last";
        public const string INIT_CHUNK_UPLOAD = "/uploads?name=:name";
        public const string UPLOAD_FILE_CHUNK = "/uploads/:uid/parts?name=:name&part=:part";
        public const string COMPLETE_FILE_UPLOAD = "/uploads/:uid?name=:name";
    }
}
