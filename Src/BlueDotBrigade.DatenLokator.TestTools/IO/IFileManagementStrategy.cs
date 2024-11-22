namespace BlueDotBrigade.DatenLokator.TestTools.IO
{
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;

	/// <summary>
	/// Responsible for locating a file on disk.
	/// </summary>
	public interface IFileManagementStrategy
	{
		void Setup(string rootDirectoryPath);

		void Setup(string rootDirectoryPath, IDictionary<string, object> testEnvironmentProperties);

		void TearDown();

		string GetFilePath(ITestNamingStrategy testNamingStrategy, string fileName, string sourceDirectory);

		string GetDefaultFilePath(string defaultFileName);
	}
}