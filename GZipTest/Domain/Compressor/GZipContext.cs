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
            }
            catch (Exception ex)
            {
                _status = Status.failed;
                doneEvent.Set();
                ConsoleInfo.ShowError($"Thread number: {i}. Message: {ex.Message}");
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
                    }
                    _queueReader.Stop();
                }
                SetStatusCompleted();
            }
            catch (Exception ex)
            {
                _status = Status.failed;
                _threadManager.Abort();
                ConsoleInfo.ShowError(ex.Message);
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
                        ConsoleInfo.ShowPercent(outStream.Position);
                    }
                }
                SetStatusCompleted();
            }
            catch (Exception ex)
            {
                _status = Status.failed;
                _threadManager.Abort();
                ConsoleInfo.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Установить статус выполненным
        /// </summary>
        private void SetStatusCompleted()
        {
            if (_status == Status.process)
            {
                _status = Status.copmleted;
            }
        }

        /// <summary>
        /// Результат выполнения комрессии 
        /// </summary>
        /// <returns>1 - ошибка, 0 - успех</returns>
        public Status GetResult()
        {
            return _status;
        }
    }
}
