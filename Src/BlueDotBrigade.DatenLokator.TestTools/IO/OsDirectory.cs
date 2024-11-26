namespace BlueDotBrigade.DatenLokator.TestTools.IO
{
    using System.IO;

    public class OsDirectory : IOsDirectory
    {
	    public OsDirectory()
	    {
		    
	    }
        public virtual bool Exists(string path) => System.IO.Directory.Exists(path);

        public virtual string[] GetDirectories(string path) => System.IO.Directory.GetDirectories(path);

        public virtual string[] GetFiles(string path) => System.IO.Directory.GetFiles(path);

        public virtual FileInfo[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return new DirectoryInfo(path).GetFiles(searchPattern, searchOption);
        }
    }
}