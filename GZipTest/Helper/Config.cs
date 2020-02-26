using GZipTest.Domain;
using System;

namespace GZipTest.Helper
{
    internal static class Config
    {
        // размер предполагаемо доступной оперативной памяти в 32 битной системе
        public const ulong AvailableBit32 = 2147483648;

        // верхняя граница размера очереди, на основе которой стоится максимальный размер
        public const ushort QueueSizeUpperBound = 500;

        // размер блока
        public const int ChunkSize = 1048576;

        // количество потоков для компресса
        public static readonly int ThreadsCount = Environment.ProcessorCount;

        // максимальный размер очереди
        public static readonly int QueueMaxSize = (new QueueSizeIdentifier()).GetMaxSize();

        public static Operation CurrOperation;
    }
}
