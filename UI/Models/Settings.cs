using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nonogram.Lib.FileCreators;

namespace Nonogram.UI.Models
{
    internal class Settings
    {
        private int _selectedFileCreator;
        
        public Settings()
        {
            FileCreators.Add(new JcdFileCreator());
            FileCreators.Add(new CwdFileCreator());
        }

        [JsonIgnore]
        public List<IFileCreator> FileCreators { get; set; } = new List<IFileCreator>();
        
        public List<string> DownloadPatterns { get; set; } = new List<string>();
        public int SelectedFileCreator
        {
            get => _selectedFileCreator;
            set
            {
                if (value >= FileCreators.Count)
                    throw new ArgumentNullException(nameof(SelectedFileCreator));
                _selectedFileCreator = value;
            }
        }
        
        [JsonIgnore]
        public IFileCreator FileCreator => FileCreators[SelectedFileCreator];
        
        public string OutputFolder { get; set; } = "";
        public bool Topmost { get; set; }
    }
}