using System;

namespace GZipTest.Helper
{
    internal static class Constant
    {
        public static readonly int ThreadsCount = Environment.ProcessorCount; 
        public const int ChunkSize = 1048576;
    }
}
