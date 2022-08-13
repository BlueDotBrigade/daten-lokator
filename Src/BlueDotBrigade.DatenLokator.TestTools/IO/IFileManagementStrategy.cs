namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System.Collections;
	using System.Collections.Specialized;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

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