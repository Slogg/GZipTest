using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest.Domain
{
    internal interface IGZipStrategy
    {
        /// <summary>
        /// Запуск процесса чтения файла
        /// </summary>
        void Read();

        /// <summary>
        /// Запуск процесса записи файла
        /// </summary>
        void Write();

        /// <summary>
        /// Обработка файла
        /// </summary>
        void Handle();
    }
}
