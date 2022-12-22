using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nonogram.Lib.FileCreators;
using Nonogram.Lib;
using Nonogram.UI.ViewModels;

namespace Nonogram.UI.Models
{
    internal class NonogramSaver
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly int _id;

        public List<string> UrlPatterns;

        public NonogramSaver(int id)
        {
            _id = id;
            FileCreator = null;
        }

        public Logger Logger { get; set; }
        public string OutputFolder { get; set; }
        public IFileCreator FileCreator { get; set; }

        public async void SaveAsync()
        {
            try
            {
                var webContent = await GetWebContentAsync();
                var rawNonogram = GetRawNonogram(webContent);

                Logger.Write("Расчет модели кроссворда ... ");
                var nonogramModel = new NonogramModel(rawNonogram);
                Logger.WriteLine("Готово.");

                PrintModelInfo(nonogramModel);
                SaveNonogramToFile(nonogramModel);
                
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void SaveNonogramToFile(NonogramModel model)
        {
            if (FileCreator is null)
                throw new ArgumentException("Не задан тип файла для сохранения.");
    
            var path = "";
            if (!string.IsNullOrEmpty(OutputFolder))
                path = OutputFolder.TrimEnd('\\') + '\\';
            var filename = $"{path}{_id}.{FileCreator.FileExtension}";

            Logger.Write($"Создание файла {filename} ... ");
            
            FileCreator.Model = model;
            FileCreator.CreateFile(filename);
            
            Logger.WriteLine("Готово.");
        }
        
        private int[][] GetRawNonogram(string webContent)
        {
            Logger.Write("Обработка данных... ");
            
            var parser = new ContentParser(webContent);
            var result = parser.GetNonogramData();
            
            Logger.WriteLine("Готово.");
           
            return result;
        }

        private async Task<string> GetWebContentAsync()
        {
            if (UrlPatterns.Count == 0)
                throw new ArgumentException("Не заданы шаблоны адресов для скачивания.");
            
            string content = null;
            foreach (var pattern in UrlPatterns)
            {
                try
                {
                    var uri = BuildUri(pattern, _id);
                    Logger.Write($"Получаю данные из {uri.AbsoluteUri} ... ");

                    var request = await httpClient.GetAsync(uri);
                    if (request.IsSuccessStatusCode)
                    {
                        Logger.WriteLine("OK.");
                        content = await request.Content.ReadAsStringAsync();
                        break;
                    }

                    Logger.WriteLine($"Ошибка: {request.StatusCode}");
                }
                catch (ArgumentException ex)
                {
                    Logger.WriteLine($"Ошибка: {ex.Message}");
                }
                
            }
            
            if (string.IsNullOrEmpty(content))
                throw new IOException("Не удалось загрузить содержимое веб-страницы.");

            return content;
        }

        private void PrintModelInfo(NonogramModel nonogram)
        {
            Logger.WriteLine();
            Logger.WriteLine("Информация и кроссворде:");
            Logger.WriteLine($"Размер по горизонтали : {nonogram.HorizontalSize}");
            Logger.WriteLine($"Размер по вертикали  : {nonogram.VerticalSize}");

            if (nonogram.HorizontalSize > 80 || nonogram.VerticalSize > 80)
                Logger.WriteLine("Внимание! Pix-a-Pix Puzzle World не поддерживает кроссворды более 80x80 клеток!");

            var colorsString = nonogram.ColorsNumber > 1
                ? $"{nonogram.ColorsNumber}"
                : "монохромный";
            Logger.WriteLine($"Цветов : {colorsString}");
            Logger.WriteLine();
        }

        private static Uri BuildUri(string patternString, int id)
        {
            const string idPattern = @"(?i)\$\(id\)";
            if (!Regex.Match(patternString, idPattern).Success)
                throw new ArgumentException("Неверно задан шаблон адреса.");

            var url = Regex.Replace(patternString, idPattern, id.ToString());

            if (!Uri.TryCreate(url, UriKind.Absolute, out var request)
                || (request.Scheme != Uri.UriSchemeHttp && request.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Неверно задан шаблон адреса.");
            
            return request;
        }
    }
}