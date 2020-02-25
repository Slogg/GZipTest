using GZipTest.Helper;
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
        private int _count;
        private int _maxSize;

        public ChunkQueue(int maxSize)
        {
            _queue = new Queue<KeyValuePair<int, byte[]>>();
            _chunkId = 0;
            _count = 0;
            _maxSize = maxSize;
        }

        /// <summary>
        /// Создание и добавление в очередь нового блока
        /// </summary>
        /// <param name="data">поток байтов</param>
        public void Enqueue(byte[] data)
        {
            lock (_queue)
            {
                while (_count >= _maxSize)
                {
                    Monitor.Wait(_queue);
                }
                _queue.Enqueue(new KeyValuePair<int, byte[]>(_chunkId, data));
                _chunkId++;
                _count++;
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
                while(_count >= _maxSize || id != _chunkId)
                {
                    Monitor.Wait(_queue);
                }
                _queue.Enqueue(chunk);
                _chunkId++;
                _count++;
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
                {
                    Monitor.Wait(_queue);
                }
                if (_queue.Count == 0)
                {
                    return default;
                }
                _count--;
                Monitor.PulseAll(_queue);
                return _queue.Dequeue();
            }
        }

        public void Stop()
        {
            lock (_queue)
            {
                Monitor.PulseAll(_queue);
            }
        }

    }
}
