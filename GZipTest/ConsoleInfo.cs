using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    public static class ConsoleInfo
    {
        public static long SizeFile;
        public static void ShowPercent(long current)
        {
            long percentage = SizeFile / 100;

            Console.CursorLeft = 5;
            Console.Write("Percent progress: " + current / percentage + "%");
        }
    }
}
