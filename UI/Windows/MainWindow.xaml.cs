using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Nonogram.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void NonogramId_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            const string pattern = @"\D";
            var matches = Regex.Matches(pattern, e.Text);
            e.Handled = matches.Count > 0;
        }

        private void NonogramId_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !IsKeyNumericOrControl(e.Key);
        }

        private static bool IsKeyNumericOrControl(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;
            if (key == Key.Delete || key == Key.Back) return true;
            if (key == Key.Left || key == Key.Right || key == Key.Up || key == Key.Down) return true;
            return false;
        }
    }
}
