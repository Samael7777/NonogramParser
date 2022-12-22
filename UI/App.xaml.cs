using System;
using System.IO;
using System.Windows;
using Nonogram.UI.Models;
using Nonogram.UI.ViewModels;
using Nonogram.UI.Windows;

namespace Nonogram.UI
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {
        private Window _mainWindow;
        private SettingsWindow _settingsWindow;
        private AboutWindow _aboutWindow;

        private readonly Settings _settings = new Settings();

       private void OnStartup(object sender, StartupEventArgs e)
        {
            LoadSettings();
            InitializeWindows();
           
            Current.MainWindow = _mainWindow;
            Current.MainWindow?.Show();
        }
        
        private void InitializeWindows()
        {
            _aboutWindow = new AboutWindow
            {
                DataContext = new AboutVm()
            };
            _settingsWindow = new SettingsWindow
            {
                DataContext = new SettingsVm(_settings)
            };
            _mainWindow = new MainWindow
            {
                DataContext = new MainVm(_settings, _settingsWindow, _aboutWindow)
            };
        }
        
        private void LoadSettings()
        {
            try
            {
                SettingsSerializer.LoadSettings(_settings);
            }
            catch(Exception ex)
            {
                if (ex.GetType() != typeof(FileNotFoundException))
                    MessageBox.Show(ex.Message, "Ошибка загрузки настроек");
                SettingsSerializer.LoadDefaultSettings(_settings);
            }
        }

        private void SaveSettings()
        {
            try
            {
                SettingsSerializer.SaveSettings(_settings);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения настроек");
            }
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            SaveSettings();
        }
    }
}
