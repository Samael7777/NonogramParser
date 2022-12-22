using System;
using Nonogram.Lib.Converters.JCD;
using OpenMcdf;

namespace Nonogram.Lib.FileCreators
{
    public class JcdFileCreator : IFileCreator
    {
        public NonogramModel Model { get; set; }
        public string FileExtension => "jcd";
        public string FileDescription => "Файл Pic-a-Pix Puzzle World (*.jcd)";
        public void CreateFile(string fileName)
        {
            if (Model == null)
                throw new ArgumentNullException(nameof(Model), "Model is not set.");
            
            using (var compoundFile = new CompoundFile())
            {
                var contents = new JcdConverter(Model).Contents;
                var stream = compoundFile.RootStorage.AddStream("Contents");
                stream.Append(contents);
                compoundFile.Save(fileName);
                compoundFile.Close();
            }
        }

        
    }
}