namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
    public interface IFile
    {
        bool Exists(string path);
        string ReadAllText(string path);
        System.IO.FileStream OpenRead(string path);

        void Delete(string path);

        void Copy(string sourceFileName, string destFileName);

        void Copy(string sourceFileName, string destFileName, bool overwrite);
    }
}