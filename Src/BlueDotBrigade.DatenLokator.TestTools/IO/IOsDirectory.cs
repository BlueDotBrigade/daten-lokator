namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
    using System.IO;

	/// <summary>
	/// Provides access to file system directories.
	/// </summary>
	/// <remarks>
	/// Interface exists to facilitate unit testing.
	/// </remarks>
    public interface IOsDirectory
    {
        bool Exists(string path);
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
        FileInfo[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    }
}