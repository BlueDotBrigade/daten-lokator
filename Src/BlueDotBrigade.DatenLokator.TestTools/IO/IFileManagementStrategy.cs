namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System.Collections;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	/// <summary>
	/// Responsible for locating a file on disk.
	/// </summary>
	public interface IFileManagementStrategy
	{
		void Setup(
			IOsDirectory directory,
			IOsFile file,
			string executingAssemblyPath,
			IDictionary testEnvironmentSettings);

		void TearDown();

		string GetFilePath(ITestNamingStrategy testNamingStrategy, string fileName, string sourceDirectory);
	}
}