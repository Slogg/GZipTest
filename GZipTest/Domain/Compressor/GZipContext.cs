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
            _status = Status.process;
        }

        /// <summary>
        /// Запустить процесс компрессии/декомпрессии
        /// </summary>
        public void Run()
        {
            (new Thread(Read)).Start();

            (new Thread(Write)).Start();

            _threadManager.Start(Handle);
            _queueReader.Stop();

            _status = Status.copmleted;
            ConsoleInfo.Completed();
        }

        private void Handle(object i)
        {
            try
            {
                ManualResetEvent doneEvent = _threadManager.GetEvents()[(int)i];
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
                ConsoleInfo.ShowError($"Thread number: {i}. Message: {ex.Message}");
                _status = Status.failed;
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
                        ConsoleInfo.ShowPercent(inputStream.Position);
                    }
                    _queueReader.Stop();
                }
            }
            catch (Exception ex)
            {
                ConsoleInfo.ShowError(ex.Message);
                _status = Status.failed;
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
                ConsoleInfo.ShowError(ex.Message);
                _status = Status.failed;
            }
        }

        /// <summary>
        /// Результат выполнения комрессии 
        /// </summary>
        /// <returns>1 - ошибка, 0 - успех</returns>
        public int GetResult()
        {
            if (_status == Status.copmleted)
                return 0;
            return 1;
        }
    }
}
