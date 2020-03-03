using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GZipTest.Helper;

namespace GZipTest.Domain.Compressor
{
    internal sealed class GZipContext
    {
        private IGZipStrategy _gZipStrategy;
        private Status _status;
        private ThreadManager _threadManager;
        private ChunkQueue _queueReader;
        private ChunkQueue _queueWriter;
        private string _inputFile;
        private string _outputFile;

        public GZipContext(IGZipStrategy gZipStrategy, string inputFile, string outputFile)
        {
            _gZipStrategy = gZipStrategy;
            _threadManager = new ThreadManager();
            _inputFile = inputFile;
            _outputFile = outputFile;
            _queueReader = new ChunkQueue(Config.QueueMaxSize);
            _queueWriter = new ChunkQueue(Config.QueueMaxSize);
        }

        /// <summary>
        /// Запустить процесс компрессии/декомпрессии
        /// </summary>
        public void Run()
        {
            _status = Status.process;
            (new Thread(Read)).Start();
            (new Thread(Write)).Start();

            _threadManager.Start(Handle);
            if (_status == Status.process)
            {
                _status = Status.copmleted;
            }
            _queueReader.Stop();
        }

        private void Handle(object i)
        {
            ManualResetEvent doneEvent = null;
            try
            {
                doneEvent = _threadManager.GetEvents()[(int)i];
                while (_status == Status.process)
                {
                    var chunk = _queueReader.Dequeue();

                    if (chunk.Equals(default(KeyValuePair<int, byte[]>)))
                    {
                        doneEvent.Set();
                        return;
                    }
                    _gZipStrategy.Handle(chunk, _queueWriter);
                }
                doneEvent.Set();
            }
            catch(ObjectDisposedException)
            {
                HandleException($"Thread number: {i}. Message: Работа потока прервана из-за непредвиденной ошибки");

            }
            catch (Exception ex)
            {
                HandleException($"Thread number: {i}. Message: {ex.Message}");
            }
        }

        private void Read()
        {
            try
            {
                using (var inputStream = new FileStream(_inputFile, FileMode.Open))
                {
                    while (inputStream.Position < inputStream.Length)
                    {
                        _gZipStrategy.Read(inputStream, _queueReader);
                        throw new Exception("fdsff");
                        ConsoleInfo.ShowPercent(inputStream.Position);
                    }
                    _queueReader.Stop();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex.Message);
            }
        }

        private void Write()
        {
            try
            {
                using (var outStream = new FileStream(_outputFile, FileMode.Append))
                {
                    while (_status == Status.process)
                    {
                        var chunk = _queueWriter.Dequeue();
                        if (chunk.Equals(default(KeyValuePair<int, byte[]>)))
                            return;
                        _gZipStrategy.Write(chunk, outStream);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex.Message);
            }
        }

        // Обработка исключений с прерыванием работы потоков
        private void HandleException(string errorMsg)
        {
            _status = Status.failed;
            _threadManager.Abort();
            ConsoleInfo.ShowError(errorMsg);
        }

        /// <summary>
        /// Результат выполнения комрессии 
        /// </summary>
        public Status GetResult()
        {
            return _status;
        }
    }
}
