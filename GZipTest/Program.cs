using GZipTest.Domain.Compressor;
using GZipTest.Helper;
using System;
using System.IO;

namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
            args = new[] { "compress", "3333.avi", "3333.avi.gz" };
            File.Delete("3333.avi.gz");
            //args = new[] { "decompress", "3333.avi.gz", "3333.avi" };
            //File.Delete("3333.avi");

            try
            {
                IGZipStrategy gZip;

                var validation = new ArgsValidation(args);
                validation.Execute();

                switch (args[0].ToLower())
                {
                    case "compress":
                        gZip = new CompressorStrategy();
                        Config.CurrOperation = Operation.compress;
                        break;
                    case "decompress":
                        gZip = new DecompressorStrategy();
                        Config.CurrOperation = Operation.decompress;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                using (FileStream inputStream = new FileStream(args[1], FileMode.Open))
                {
                    ConsoleInfo.SizeFile = inputStream.Length;
                }
                var context = new GZipContext(gZip, args[1], args[2]);
                context.Run();
                return context.GetResult();

            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }
        }
    }
}
