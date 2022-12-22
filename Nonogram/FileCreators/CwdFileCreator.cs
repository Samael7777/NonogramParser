using System.IO;
using Nonogram.Lib.Converters.CWD;

namespace Nonogram.Lib.FileCreators
{
    public class CwdFileCreator : IFileCreator
    {
        public NonogramModel Model { get; set; }
        public string FileExtension => "cwd";
        public string FileDescription => "Файл Намкхоева Владимира (*.cwd)";

        public void CreateFile(string fileName)
        {
            using (var fw = new StreamWriter(fileName))
            {
                var content = new CwdConverter(Model).GetContent();
                fw.Write(content);
                fw.Flush();
            }
        }
    }
}