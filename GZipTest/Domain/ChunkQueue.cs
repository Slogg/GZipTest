using System.Collections.Generic;
using System.Threading;

namespace GZipTest.Domain
{
    /// <summary>
    /// Очередь с блоками данных
    /// </summary>
    internal sealed class ChunkQueue
    {
        private Queue<KeyValuePair<int, byte[]>> _queue;
        private int _chunkId;

        public ChunkQueue()
        {
            _queue = new Queue<KeyValuePair<int, byte[]>>();
            _chunkId = 0;
        }

        /// <summary>
        /// Создание и добавление в очередь нового блока
        /// </summary>
        /// <param name="data">поток байтов</param>
        public void Enqueue(byte[] data)
        {
            lock (_queue)
            {
                _queue.Enqueue(new KeyValuePair<int, byte[]>(_chunkId, data));
                _chunkId++;
                Monitor.PulseAll(_queue);
            }
        }

        /// <summary>
        /// Добавление в очередь нового блока
        /// </summary>
        /// <param name="data">поток байтов</param>
        public void Enqueue(KeyValuePair<int, byte[]> chunk)
        {
            int id = chunk.Key;
            lock (_queue)
            {
                while(id != _chunkId)
                {
                    Monitor.Wait(_queue);
                }
                _queue.Enqueue(chunk);
                _chunkId++;
                Monitor.PulseAll(_queue);
            }
        }

        /// <summary>
        /// Удалить блок из очереди
        /// </summary>
        /// <returns>default - если элементов не осталось</returns>
        public KeyValuePair<int, byte[]> Dequeue()
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                    Monitor.Wait(_queue);
                if (_queue.Count == 0)
                    return default;
                return _queue.Dequeue();
            }
        }

    }
}
