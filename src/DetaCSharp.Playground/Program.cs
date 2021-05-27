using System;
using System.IO;
using System.Linq;

namespace DetaCSharp.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var deta = new Deta("PROJECT_KEY_HERE");
                var drive = deta.Drive("test");

                var putOptions = new Drive.PutOptions();
                putOptions.SetData($"string text file {DateTime.Now}");
                var string_result = drive.Put($"string_text_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(string_result);

                putOptions = new Drive.PutOptions();
                putOptions.Path = ".\\files\\text_file.txt";
                var text_result = drive.Put($"text_file_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(text_result.Name);


                putOptions.Path = ".\\files\\deta_logo.svg";
                var image_result = drive.Put($"image_file_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(image_result.Name);


                putOptions.Path = ".\\files\\11MB_file.xpto";
                var large_result = drive.Put($"large_file_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(large_result.Name);



                var list = drive.List().Result;


                Console.WriteLine(string.Join("\n", list.Names.ToList()));


                using var get_text = drive.Get(text_result.Name).Result;
                using var textStream = new FileStream(".\\get_text.txt", FileMode.Create);
                get_text.CopyTo(textStream);
                textStream.Flush();

                using var get_string = drive.Get(string_result.Name).Result;
                using var stringStream = new FileStream(".\\get_string.txt", FileMode.Create);
                get_string.CopyTo(stringStream);
                stringStream.Flush();

                using var get_image = drive.Get(image_result.Name).Result;
                using var imageStream = new FileStream(".\\get_image.svg", FileMode.Create);
                get_image.CopyTo(imageStream);
                imageStream.Flush();


                using var get_large = drive.Get(large_result.Name).Result;
                using var largeStream = new FileStream(".\\get_larte.xpto", FileMode.Create);
                get_large.CopyTo(largeStream);
                largeStream.Flush();


                var delete_Response = drive.DeleteMany(list.Names).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Aperte uma tecla para continuar");
                Console.ReadKey();
            }
        }
    }
}
