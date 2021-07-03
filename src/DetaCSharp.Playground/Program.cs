using DetaCSharp.Base;
using DetaCSharp.Types;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DetaCSharp.Playground
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var deta = new Deta("b0j773yp_uhdP1Wg7MsHbR7fkrocRWHUHZAsvDPmB");


                var db = deta.Base("test");
                Task.Run(async () =>
                {
                    //var lista = db.Fetch<JsonElement>(Array.Empty<object>(), pages: 5, buffer: 3);

                    ////mode 1
                    //await foreach (var item in lista)
                    //{
                    //    Console.WriteLine(item.GetProperty("key"));
                    //}

                    ////mode 2
                    //var enumerator = lista.GetAsyncEnumerator();
                    //while (await enumerator.MoveNextAsync())
                    //{
                    //    Console.WriteLine(enumerator.Current.GetProperty("key"));
                    //}

                }).Wait();


                var item = new TestClass
                {
                    Key = "666",
                    Obj = new InnerTestClass
                    {
                        Valor = 10,
                        Data = DateTime.Now,
                        Collection = new[] { "1", "2" }
                    },
                    Pi = 3.14f,
                    Numero = 10,
                    Collection = new[] { 3 }
                };

                var list = new object[]
                {
                    new
                    {
                        Test = 10,
                        Foo = "Bar"
                    },
                    new
                    {
                        Prop1 = "value1",
                        Prop2 = 10
                    }
                };

                var keys2 = db.Put(list).Result;

                Console.WriteLine(string.Join(",", keys2));

                var keys = db.Put(item, item.Key).Result;

                Console.WriteLine(string.Join(",", keys));



                //update with sugar

                var update = db.CreateUpdate();
                update[ActionTypes.Increment]["Numero"] = 2; //increment by dictionary
                update.Set["NewProperty"] = 20; ////increment by dictionary mode 2
                //fluent api and fluent typed api
                update.With(ActionTypes.Set, "Obj.NewSubProp", "somevalue")
                      .WithDelete("Obj.Valor")
                      .WithAppend("Obj.Collection", new[] { "2" }) //will add 2 to Obj.Collection 
                      .WithAppend<TestClass>(t => t.Obj.Collection, 3) //will ass 3 to Obj.Collection will be 2 and 3 appended
                      .WithAppend<TestClass>(t => t.Collection, new[] { 1, 2, 4 });


                var key3 = db.Update(update, "666").Result;

                Console.WriteLine(key3.Key);


                //update with utils ans raw
                //on C# we can´t do a proeprty on string or with dot like "Obj.Valor"
                var updareRaw = new
                {
                    Numero = BaseUtils.Increment(2),
                    Collection = BaseUtils.Append(666)
                };







                //var drive = deta.Drive("test");

                //var putOptions = new Drive.PutOptions();
                //putOptions.SetData($"string text file {DateTime.Now}");
                //var string_result = drive.Put($"string_text_{DateTime.Now.Ticks}", putOptions).Result;
                //Console.WriteLine(string_result);

                //putOptions = new Drive.PutOptions();
                //putOptions.Path = ".\\files\\text_file.txt";
                //var text_result = drive.Put($"text_file_{DateTime.Now.Ticks}", putOptions).Result;
                //Console.WriteLine(text_result.Name);


                //putOptions.Path = ".\\files\\deta_logo.svg";
                //var image_result = drive.Put($"image_file_{DateTime.Now.Ticks}", putOptions).Result;
                //Console.WriteLine(image_result.Name);


                //putOptions.Path = ".\\files\\11MB_file.xpto";
                //var large_result = drive.Put($"large_file_{DateTime.Now.Ticks}", putOptions).Result;
                //Console.WriteLine(large_result.Name);



                //var list = drive.List().Result;


                //Console.WriteLine(string.Join("\n", list.Names.ToList()));


                //using var get_text = drive.Get(text_result.Name).Result;
                //using var textStream = new FileStream(".\\get_text.txt", FileMode.Create);
                //get_text.CopyTo(textStream);
                //textStream.Flush();

                //using var get_string = drive.Get(string_result.Name).Result;
                //using var stringStream = new FileStream(".\\get_string.txt", FileMode.Create);
                //get_string.CopyTo(stringStream);
                //stringStream.Flush();

                //using var get_image = drive.Get(image_result.Name).Result;
                //using var imageStream = new FileStream(".\\get_image.svg", FileMode.Create);
                //get_image.CopyTo(imageStream);
                //imageStream.Flush();


                //using var get_large = drive.Get(large_result.Name).Result;
                //using var largeStream = new FileStream(".\\get_larte.xpto", FileMode.Create);
                //get_large.CopyTo(largeStream);
                //largeStream.Flush();


                //var delete_Response = drive.DeleteMany(list.Names).Result;
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

        public class TestClass
        {
            public string Key { get; set; }
            public string Teste { get; set; }
            public float Pi { get; set; }
            public int Numero { get; set; }
            public InnerTestClass Obj { get; set; }

            public int[] Collection { get; set; }
        }

        public class InnerTestClass
        {
            public int Valor { get; set; }
            public string Str { get; set; }
            public DateTime Data { get; set; }

            public string[] Collection { get; set; }
        }
    }
}
