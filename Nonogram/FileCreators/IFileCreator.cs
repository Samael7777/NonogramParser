namespace Nonogram.Lib.FileCreators
{
    public interface IFileCreator
    {
        NonogramModel Model { get; set; }
        void CreateFile(string fileName);
        string FileExtension { get; }
        string FileDescription { get; }
    }
}