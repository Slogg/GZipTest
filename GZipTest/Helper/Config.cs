using Microsoft.VisualBasic.Devices;
using System;

namespace GZipTest.Helper
{
    internal sealed class Config
    {
        private static Config _instance;
        private const ulong availableBit32 = 2147483648;

        public const int ChunkSize = 1048576;
        public readonly int ThreadsCount = Environment.ProcessorCount;
        public int QueueMaxSize { get; private set; }

        private Config()
        {
            CalcFields();
        }

        public static Config Get()
        {
            _instance ??= new Config();
            return _instance;
        }

        // Расчитать поля для использования 
        private void CalcFields()
        {
            var availablePhys = AvailablePhysical(); 
            QueueMaxSize = 500 > availablePhys ? availablePhys : 500;
        }

        // Доступное кол-во памяти, которое можно выделить для данных
        private int AvailablePhysical()
        {
            ulong availableMem = Environment.Is64BitProcess 
                ? new ComputerInfo().AvailablePhysicalMemory 
                : availableBit32;
            return (int)(availableMem / 2 / ChunkSize);
        }

    }
}
