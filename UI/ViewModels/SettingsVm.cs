using System;
using System.Linq;
using System.Text;
using Nonogram.UI.Models;

namespace Nonogram.UI.ViewModels
{
    internal class SettingsVm : BaseViewModel
    {
        private readonly Settings _settings;
        public SettingsVm(Settings settings)
        {
            _settings = settings;
            DefaultCmd = new RelayCommand((_) => SetDefaultSettings());
        }

        private void SetDefaultSettings()
        {
            SettingsSerializer.LoadDefaultSettings(_settings);
            OnPropertyChanged("");
        }

        public RelayCommand DefaultCmd { get; }

        public string[] FileTypes => _settings.FileCreators.Select(fc=>fc.FileDescription).ToArray();

        public int FileCreatorIndex
        {
            get => _settings.SelectedFileCreator;
            set
            {
                if (value == _settings.SelectedFileCreator) return;
                _settings.SelectedFileCreator = value;
                OnPropertyChanged(nameof(FileCreatorIndex));
            }
        }

        public string Patterns
        {
            get => PatternsToText();
            set
            {
                TextToPatterns(value);
                OnPropertyChanged(nameof(Patterns));
            }
        }

        private string PatternsToText()
        {
            var text = new StringBuilder();
            foreach (var pattern in _settings.DownloadPatterns)
            {
                text.AppendLine(pattern);
            }
            return text.ToString();
        }

        private void TextToPatterns(string text)
        {
            _settings.DownloadPatterns.Clear();
            var lines = text.Split(new []{'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var pattern = line.Trim();
                if (pattern.Length >= 1)
                    _settings.DownloadPatterns.Add(line);
            }
        }
    }
}
