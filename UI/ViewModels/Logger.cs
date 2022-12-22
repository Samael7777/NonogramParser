using System;

namespace Nonogram.UI.ViewModels
{
    internal class Logger
    {
        private readonly Action<string> _write;
        public Logger(Action<string> write)
        {
            _write = write;
        }

        public void Write(string message)
        {
            _write(message);
        }

        public void WriteLine(string message = "")
        {
            Write(message);
            Write(Environment.NewLine);
        }
    }
}
