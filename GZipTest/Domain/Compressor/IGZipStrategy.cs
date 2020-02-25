using System.Collections.Generic;
using System.IO;

namespace GZipTest.Domain.Compressor
{
    internal interface IGZipStrategy
    {
        /// <summary>
        /// Обработка файла
        /// </summary>
        /// <param name="chunk">блок данных</param>
        /// <param name="queueWriter">пул для записи</param>
        void Handle(KeyValuePair<int, byte[]> chunk, ChunkQueue queueWriter);

        /// <summary>
        /// Запуск процесса чтения файла
        /// </summary>
        /// <param name="inputStream">поток данных</param>
        /// <param name="queueReader">пул для чтения</param>
        void Read(FileStream inputStream, ChunkQueue queueReader);

        /// <summary>
        /// Запуск процесса записи файла
        /// </summary>
        /// <param name="chunk">блок данных</param>
        /// <param name="outStream">поток данных</param>
        void Write(KeyValuePair<int, byte[]> chunk, FileStream outStream);
    }
}
