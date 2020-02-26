using GZipTest.Helper;
using Microsoft.VisualBasic.Devices;
using System;

namespace GZipTest.Domain
{
    internal sealed class QueueSizeIdentifier
    {

        /// <summary>
        /// Получить максимальную размерность для очереди
        /// </summary>
        public int GetMaxSize()
        {
            var availablePhys = AvailablePhysical();
            var querySize = (int)(availablePhys / 2 / Config.ChunkSize);
            return Config.QueueSizeUpperBound > querySize ? querySize : Config.QueueSizeUpperBound;
        }

        // Доступное кол-во памяти, которое можно выделить для данных
        private ulong AvailablePhysical()
        {
            var availableMem = new ComputerInfo().AvailablePhysicalMemory;
            // Ограничить под 32 битную систему, если памяти доступно больше
            if (!Environment.Is64BitProcess)
            {
                availableMem = availableMem > Config.AvailableBit32 ? Config.AvailableBit32 : availableMem;
            }
            return availableMem;
        }
    }
}
