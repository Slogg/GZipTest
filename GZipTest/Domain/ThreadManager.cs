using GZipTest.Helper;
using System;
using System.Threading;

namespace GZipTest.Domain
{
    /// <summary>
    /// Менеджер потоков
    /// </summary>
    internal sealed class ThreadManager
    {
        private readonly ManualResetEvent[] _manualEvent;
        private Thread[] _threads;

        public ThreadManager()
        {
            _manualEvent = new ManualResetEvent[Constant.ThreadsCount];
            _threads = new Thread[Constant.ThreadsCount];
        }

        /// <summary>
        /// Вызвать 
        /// </summary>
        /// <param name="startAct">метод для запуска</param>
        public void Start(Action<object> startAct)
        {
            for (int i = 0; i < Constant.ThreadsCount; i++)
            {
                var start = new ParameterizedThreadStart(obj => startAct(obj));
                _threads[i] = new Thread(start);
                _manualEvent[i] = new ManualResetEvent(false);
                _threads[i].Start(i);
            }
        }

        /// <summary>
        /// Получить список сигнал-событий
        /// </summary>
        /// <returns></returns>
        public ManualResetEvent[] GetEvents()
        {
            return _manualEvent;
        }

        /// <summary>
        /// Дождаться окончания работы
        /// </summary>
        public void WaitAll()
        {
            WaitHandle.WaitAll(_manualEvent);
        }
    }
}
