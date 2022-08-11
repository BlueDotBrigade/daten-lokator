namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System.Collections.Specialized;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	public interface IFileManager
	{
		void Setup(
			IOsDirectory directory,
			IOsFile file,
			NameValueCollection applicationSettings,
			string executingAssemblyPath);

		void TearDown();

		string GetFilePath(ITestNamingStrategy testNamingStrategy, string fileName, string sourceDirectory);
	}
}