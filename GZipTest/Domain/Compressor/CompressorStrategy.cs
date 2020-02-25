using GZipTest.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Domain.Compressor
{
    internal sealed class CompressorStrategy : IGZipStrategy
    {
        /// <summary>
        /// <see cref="IGZipStrategy.Handle"/>
        /// </summary>
        public void Handle(KeyValuePair<int, byte[]> chunk, ChunkQueue queueWriter)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream cs = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    cs.Write(chunk.Value, 0, chunk.Value.Length);
                    cs.Flush();

                }

                byte[] compresData = memoryStream.ToArray();
                var _out = new KeyValuePair<int, byte[]>(chunk.Key, compresData);
                queueWriter.Enqueue(_out);

            }
        }

        /// <summary>
        /// <see cref="IGZipStrategy.Read"/>
        /// </summary>
        public void Read(FileStream fileStream, ChunkQueue queueReader)
        {
            var diff = fileStream.Length - fileStream.Position;
            var bytesRead = diff <= Config.ChunkSize ? (int)diff : Config.ChunkSize;

            var lastBuffer = new byte[bytesRead];
            fileStream.Read(lastBuffer, 0, bytesRead);
            queueReader.Enqueue(lastBuffer);
        }

        /// <summary>
        /// <see cref="IGZipStrategy.Write"/>
        /// </summary>
        public void Write(KeyValuePair<int, byte[]> chunk, FileStream fileStream)
        {
            BitConverter.GetBytes(chunk.Value.Length).CopyTo(chunk.Value, 4);
            fileStream.Write(chunk.Value, 0, chunk.Value.Length);
        }
    }
}
