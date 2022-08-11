namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	/// <summary>
	/// Provides access to file system files.
	/// </summary>
	/// <remarks>
	/// Interface exists to facilitate unit testing.
	/// </remarks>
	public interface IOsFile
    {
        bool Exists(string path);
        string ReadAllText(string path);
        System.IO.FileStream OpenRead(string path);

        void Delete(string path);

        void Copy(string sourceFileName, string destFileName);

        void Copy(string sourceFileName, string destFileName, bool overwrite);
    }
}