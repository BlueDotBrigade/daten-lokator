namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	/// <summary>
	/// Uses the method name to determine what the input file is expected to be called.
	/// </summary>
	public interface ITestNamingStrategy
	{
		/// <summary>
		/// Tries to determine the name of the input file.
		/// </summary>
		/// <param name="methodOrFileName">Represents the automated test's method name, or an explicitly referenced filename.</param>
		/// <param name="fileName">Represents the output of this method call.</param>
		bool TryGetFileName(string methodOrFileName, out string fileName);
	}
}