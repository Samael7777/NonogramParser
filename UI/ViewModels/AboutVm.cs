using System.Windows;

namespace Nonogram.UI.ViewModels
{
    internal class AboutVm
    {
        public AboutVm()
        {
            CopyToClipboardCmd = new RelayCommand((txt) => CopyToClipboard((string)txt));
        }
        public RelayCommand CopyToClipboardCmd { get; }

        private void CopyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
