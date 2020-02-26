using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GZipTest.Domain.Compressor
{
    /// <summary>
    /// Decompressor
    /// </summary>
    internal sealed class DecompressorStrategy : IGZipStrategy
    {
        private int counter;

        /// <summary>
        /// <see cref="IGZipStrategy.Handle"/>
        /// </summary>
        public void Handle(KeyValuePair<int, byte[]> chunk, ChunkQueue queueWriter)
        {
            var dataSize = BitConverter.ToInt32(chunk.Value, chunk.Value.Length - 4);
            var lastBuffer = new byte[dataSize];

            using (MemoryStream ms = new MemoryStream(chunk.Value))
            {
                using (GZipStream _gz = new GZipStream(ms, CompressionMode.Decompress))
                {
                    var keyVal = new KeyValuePair<int, byte[]>(chunk.Key, lastBuffer);
                    _gz.Read(keyVal.Value, 0, keyVal.Value.Length);
                    var decompressedData = keyVal.Value.ToArray();
                    var chunkDec = new KeyValuePair<int, byte[]>(keyVal.Key, decompressedData);
                    queueWriter.Enqueue(chunkDec);
                }
            }
        }

        /// <summary>
        /// <see cref="IGZipStrategy.Read"/>
        /// </summary>
        public void Read(FileStream inputStream, ChunkQueue queueReader) 
        {
            var bufLenght = new byte[8];
            inputStream.Read(bufLenght, 0, bufLenght.Length);
            var chunkLenght = BitConverter.ToInt32(bufLenght, 4);
            byte[] compresData = new byte[chunkLenght];
            bufLenght.CopyTo(compresData, 0);
            inputStream.Read(compresData, 8, chunkLenght - 8);

            var chunk = new KeyValuePair<int, byte[]>(counter, compresData);
            queueReader.Enqueue(chunk);
            counter++;
        }

        /// <summary>
        /// <see cref="IGZipStrategy.Write"/>
        /// </summary>
        public void Write(KeyValuePair<int, byte[]> chunk, FileStream outStream)
        {
            outStream.Write(chunk.Value, 0, chunk.Value.Length);
        }
    }
}
