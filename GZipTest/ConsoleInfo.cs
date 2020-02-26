using GZipTest.Helper;
using System;

namespace GZipTest
{
    public static class ConsoleInfo
    {
        public static long SizeFile;
        public static void ShowPercent(long current)
        {
            long percentage = current / (SizeFile / 100);

            Console.CursorLeft = 0;
            Console.Write("Percent progress: " + percentage + "%");
            if (percentage == 100)
            {
                Console.WriteLine();
                Console.Write("Ending...");
            }
        }

        public static void Completed()
        {
            Console.CursorLeft = 0;
            Console.Write("Successful " + Config.CurrOperation.ToString());
        }

        public static void ShowError(string message)
        {
            Console.WriteLine($"Error: {message}");
        }
    }
}
