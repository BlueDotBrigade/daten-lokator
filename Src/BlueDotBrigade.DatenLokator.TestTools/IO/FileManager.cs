namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System;
	using System.Collections;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	internal class FileManager
	{
		private readonly IFileManagementStrategy _fileManagementStrategy;
		private readonly ITestNamingStrategy _testNamingStrategy;

		private bool _isIniitialized;

		public FileManager(IFileManagementStrategy fileManagementStrategy, ITestNamingStrategy testNamingStrategy)
		{
			_fileManagementStrategy = fileManagementStrategy;
			_testNamingStrategy = testNamingStrategy;
		}

		public void Setup(
			IOsDirectory directory,
			IOsFile file,
			string executingAssemblyPath,
			IDictionary testEnvironmentSettings)
		{
			_fileManagementStrategy.Setup(directory, file, executingAssemblyPath, testEnvironmentSettings);
			_isIniitialized = true;
		}

		public void TearDown()
		{
			_fileManagementStrategy.TearDown();
		}

		public string GetFilePath(string fileName, string sourceDirectory)
		{
			if (_isIniitialized)
			{
				return _fileManagementStrategy.GetFilePath(_testNamingStrategy, fileName, sourceDirectory);
			}
			else
			{
				throw new InvalidOperationException(
					$"The {nameof(FileManager)} has not been initialized. Hint: Call Setup() method");
			}
		}
	}
}
