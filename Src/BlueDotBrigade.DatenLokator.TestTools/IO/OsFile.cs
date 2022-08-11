namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
    using System.IO;

	public class OsFile : IOsFile
    {
        public virtual bool Exists(string path) => System.IO.File.Exists(path);

        public virtual string ReadAllText(string path) => System.IO.File.ReadAllText(path);

        public virtual FileStream OpenRead(string path) => System.IO.File.OpenRead(path);

        public virtual void Delete(string path) => System.IO.File.Delete(path);

        public virtual void Copy(string sourceFileName, string destFileName) =>
            System.IO.File.Copy(sourceFileName, destFileName);

        public virtual void Copy(string sourceFileName, string destFileName, bool overwrite) =>
            System.IO.File.Copy(sourceFileName, destFileName, overwrite);
    }
}