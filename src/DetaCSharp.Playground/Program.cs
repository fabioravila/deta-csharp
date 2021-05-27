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
                var deta = new Deta("");
                var drive = deta.Drive("test");

                var putOptions = new Drive.PutOptions();
                putOptions.SetData($"string text file {DateTime.Now}");
                var string_result = drive.Put($"string_text_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(string_result);

                putOptions = new Drive.PutOptions();
                putOptions.Path = ".\\files\\text_file.txt";
                var text_result = drive.Put($"text_file_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(text_result);


                putOptions.Path = ".\\files\\deta_logo.svg";
                var image_result = drive.Put($"image_file_{DateTime.Now.Ticks}", putOptions).Result;
                Console.WriteLine(image_result);

                var list = drive.List().Result;


                Console.WriteLine(string.Join("\n", list.Names.ToList()));


                using var get_text = drive.Get(text_result).Result;
                using var textStream = new FileStream(".\\get_text.txt", FileMode.Create);
                get_text.CopyTo(textStream);

                using var get_string = drive.Get(string_result).Result;
                using var stringStream = new FileStream(".\\get_string.txt", FileMode.Create);
                get_string.CopyTo(stringStream);

                using var get_image = drive.Get(image_result).Result;
                using var imageStream = new FileStream(".\\get_image.svg", FileMode.Create);
                get_image.CopyTo(imageStream);
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
