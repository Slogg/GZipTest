using GZipTest.Helper;
using GZipTest.Properties;
using System;

namespace GZipTest
{
    /// <summary>
    /// Валидация входных параметров
    /// </summary>
    internal sealed class ArgsValidation
    {
        private string[] _args;

        public ArgsValidation(string[] args)
        {
            _args = args;
        }

        public void Execute()
        {
            HandleCondition(() => _args.Length == 3, String.Format(
                Errors.ErrInvalidCommand,
                Operation.compress.ToString(),
                Operation.decompress.ToString())); 
            HandleCondition(IsValidFirstArg, String.Format(
                Errors.ErrFirstArg, 
                Operation.compress.ToString(), 
                Operation.decompress.ToString()));
            HandleCondition(() => _args[1].Length != 0, Errors.ErrInvalidInputFile);
            HandleCondition(() => _args[1].Length != 0, Errors.ErrInvalidOutputFile);
            HandleCondition(() => _args[1] != _args[2], Errors.ErrFileNamesEqual);
        }

        // Обработка условия. При false - выбросить исключение с заданным в аргументах сообщением
        private void HandleCondition(Func<bool> condition, string errorMsg)
        {
            if (!condition())
            {
                throw new Exception(errorMsg);
            };
        }

        private bool IsValidFirstArg()
        {
            return _args[0].ToLower() == Operation.compress.ToString().ToLower()
                || _args[0].ToLower() == Operation.decompress.ToString().ToLower();
        }

    }
}
