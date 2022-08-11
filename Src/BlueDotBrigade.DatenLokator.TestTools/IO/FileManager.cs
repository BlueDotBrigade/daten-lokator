namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System;
	using System.Collections.Specialized;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	internal class FileManager
	{
		private readonly IFileManager _fileManager;
		private readonly ITestNamingStrategy _testNamingStrategy;

		private bool _isIniitialized;

		public FileManager(IFileManager fileManager, ITestNamingStrategy testNamingStrategy)
		{
			_fileManager = fileManager;
			_testNamingStrategy = testNamingStrategy;
		}

		public void Setup(
			IOsDirectory directory,
			IOsFile file,
			NameValueCollection applicationSettings,
			string executingAssemblyPath)
		{
			_fileManager.Setup(directory, file, applicationSettings, executingAssemblyPath);
			_isIniitialized = true;
		}

		public void TearDown()
		{
			_fileManager.TearDown();
		}

		public string GetFilePath(string fileName, string sourceDirectory)
		{
			if (_isIniitialized)
			{
				return _fileManager.GetFilePath(_testNamingStrategy, fileName, sourceDirectory);
			}
			else
			{
				throw new InvalidOperationException(
					$"The {nameof(FileManager)} has not been initialized. Hint: Call Setup() method");
			}
		}
	}
}
