using System;
using System.IO;
using Newtonsoft.Json;

namespace Nonogram.UI.Models
{
    internal static class SettingsSerializer
    {
        private const string SettingsFileName = "Settings.json";
        private static readonly JsonSerializerSettings jsonSettings;
        
        static SettingsSerializer()
        {
            jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
        }

        public static void LoadDefaultSettings(Settings settings)
        {
            settings.OutputFolder = AppDomain.CurrentDomain.BaseDirectory;
            settings.DownloadPatterns.Clear();
            settings.DownloadPatterns.Add( @"https://www.nonograms.ru/nonograms/i/$(Id)");
            settings.DownloadPatterns.Add( @"https://www.nonograms.ru/nonograms2/i/$(Id)");
            settings.SelectedFileCreator = 0;
        }

        public static void LoadSettings(Settings settings)
        {
            if (!File.Exists(SettingsFileName))
                throw new FileNotFoundException(SettingsFileName);

            string json;
            using (var reader = new StreamReader(SettingsFileName))
            {
                json = reader.ReadToEnd();
                reader.Close();
            }

            if (string.IsNullOrEmpty(json))
                throw new FileLoadException("Не удалось прочитать файл настроек.");

            var storedSettings = JsonConvert.DeserializeObject<Settings>(json, jsonSettings);
            if (storedSettings == null)
                throw new JsonReaderException("Не корректный файл настроек.");

            SetNewSettings(storedSettings, settings);
        }

        public static void SaveSettings(Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings, jsonSettings);
            if (string.IsNullOrEmpty(json))
                throw new JsonSerializationException("Ошибка сериализации настроек");

            using (var writer = new StreamWriter(SettingsFileName, false))
            {
                writer.Write(json);
                writer.Flush();
                writer.Close();
            }
        }

        private static void SetNewSettings(Settings source, Settings destination)
        {
            var properties = typeof(Settings).GetProperties();
            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(JsonIgnoreAttribute))) continue;
                
                var value = property.GetValue(source);
                property.SetValue(destination, value);
            }
        }
    }
}
