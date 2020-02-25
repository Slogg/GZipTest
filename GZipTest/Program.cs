using GZipTest.Domain.Compressor;
using System;
using System.Diagnostics;
using System.IO;

namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            args = new[] { "compress", "3333.7z", "3333.7z.gz" };
            File.Delete("3333.7z.gz");
#endif
//#if DEBUG
//            args = new[] { "decompress", "3333.7z.gz", "3333.7z" };
//            File.Delete("3333.7z");
//#endif
            IGZipStrategy gZip;


            var sw = new Stopwatch();
            sw.Start();

            try
            {
                switch (args[0].ToLower())
                {
                    case "compress":
                        gZip = new CompressorStrategy();
                        break;
                    case "decompress":
                        gZip = new DecompressorStrategy();
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

                //Console.WriteLine(sw.ElapsedMilliseconds);
                //Console.ReadKey();
                return context.GetResult();

            }

            catch (Exception ex)
            {
                // ToDo: throws
                return 1;
            }
        }
    }
}
