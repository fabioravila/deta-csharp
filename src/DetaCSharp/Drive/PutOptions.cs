using System.IO;

namespace DetaCSharp.Drive
{
    public class PutOptions
    {
        public object Data { get; private set; }
        public string Path { get; set; }
        public string ContentType { get; set; }


        public void SetData(Stream stream)
        {
            Data = stream;
        }

        public void SetData(byte[] byteArray)
        {
            Data = byteArray;
        }


        public void SetData(string data)
        {
            Data = data;
        }
    }
}
