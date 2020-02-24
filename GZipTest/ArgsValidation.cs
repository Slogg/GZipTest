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
            HandleCondition(IsMatchPattern, Resources.ErrCantAccessFile);

        }

        // Обработка условия. При false - выбросить исключение с заданным в аргументах сообщением
        private void HandleCondition(Func<bool> condition, string errorMsg)
        {
            if (!condition())
            {
                throw new Exception(errorMsg);
            };
        }

        #region Conditions
        private bool IsMatchPattern()
        {
            return _args.Length == 3;
        }
        #endregion
    }
}
