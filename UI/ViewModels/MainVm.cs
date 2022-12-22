using System;
using System.Windows;
using Nonogram.UI.Models;
using Nonogram.UI.Windows;
using Ookii.Dialogs.Wpf;

namespace Nonogram.UI.ViewModels
{
    internal class MainVm : BaseViewModel
    {
        private readonly Logger _logger;
        private readonly Settings _settings;
        private readonly SettingsWindow _settingsWindow;
        private readonly AboutWindow _aboutWindow;
        private string _log;
        private string _nonogramId;

        public MainVm(Settings settings, SettingsWindow settingsWindow, AboutWindow aboutWindow)
        {
            _log = "";
            _nonogramId = "";
            _settings = settings;
            _settingsWindow = settingsWindow;
            _aboutWindow = aboutWindow;

            InitializeCommands();

            void WriteLog(string msg)
            {
                Application.Current.Dispatcher.Invoke(() => { Log += msg; });
            }

            _logger = new Logger(WriteLog);
        }

        public RelayCommand SettingsCmd { get; private set; }
        public RelayCommand AboutCmd { get; private set; }
        public RelayCommand ExitCmd { get; private set; }
        public RelayCommand BrowseCmd { get; private set; }
        public RelayCommand SaveCmd { get; private set; }
        public RelayCommand CleanLogCmd { get; private set; }

        public string Log
        {
            get => _log;
            set => SetField(ref _log, value, nameof(Log));
        }

        public string OutputFolder
        {
            get => _settings.OutputFolder;
            set
            {
                _settings.OutputFolder = value;
                OnPropertyChanged(nameof(OutputFolder));
            }
        }

        public string NonogramId
        {
            get => _nonogramId;
            set => SetField(ref _nonogramId, value, nameof(NonogramId));
        }

        public bool Topmost
        {
            get => _settings.Topmost;
            set
            {
                if (_settings.Topmost == value) return;
                _settings.Topmost = value;
                OnPropertyChanged(nameof(Topmost));
            }
        }

        private void InitializeCommands()
        {
            SettingsCmd = new RelayCommand(_ => _settingsWindow.Show());
            AboutCmd = new RelayCommand(_ => _aboutWindow.Show());
            ExitCmd = new RelayCommand(_ => Application.Current.Shutdown());
            BrowseCmd = new RelayCommand(_ => SelectOutputFolder());
            SaveCmd = new RelayCommand(_ => SaveNonogram());
            CleanLogCmd = new RelayCommand(_ => { Log = ""; });
        }



        private void SelectOutputFolder()
        {
            var selFolderDlg = new VistaFolderBrowserDialog
            {
                Description = @"Выберите папку для сохранения кроссвордов",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
                Multiselect = false
            };

            var currentPath = string.IsNullOrEmpty(OutputFolder)
                ? AppDomain.CurrentDomain.BaseDirectory
                : OutputFolder;

            currentPath = currentPath.TrimEnd('\\') + "\\";
            selFolderDlg.SelectedPath = currentPath;
            if (selFolderDlg.ShowDialog() ?? false) OutputFolder = selFolderDlg.SelectedPath;
        }

        private void SaveNonogram()
        {
            if (!int.TryParse(_nonogramId, out var id))
            {
                _logger.WriteLine("Id кроссворда не корректен!");
                return;
            }

            var saver = new NonogramSaver(id)
            {
                FileCreator = _settings.FileCreator,
                OutputFolder = OutputFolder,
                Logger = _logger,
                UrlPatterns = _settings.DownloadPatterns
            };
            saver.SaveAsync();
        }
    }
}