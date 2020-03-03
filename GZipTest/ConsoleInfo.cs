using GZipTest.Helper;
using System;

namespace GZipTest
{
    public static class ConsoleInfo
    {
        public static long SizeFile;

        /// <summary>
        /// Информация о проценте выполнения компрессии
        /// </summary>
        /// <param name="current">текущее положение в массиве байт</param>
        public static void ShowPercent(long current)
        {
            long percentage = current / (SizeFile / 100);

            Console.CursorLeft = 0;
            Console.Write("Percent progress: " + percentage + "%");
        }

        /// <summary>
        /// Информационное сообщение о завршении процесса
        /// </summary>
        /// <param name="status">статус выполнения операции</param>
        public static void ShowResult(Status status)
        {
            Console.WriteLine("Completed with code: " + (int)status);
        }

        /// <summary>
        /// Отобразить текст ошибки
        /// </summary>
        /// <param name="message">текст ошибки</param>
        public static void ShowError(string message)
        {
            Console.WriteLine("\nError: " + message);
        }
    }
}
