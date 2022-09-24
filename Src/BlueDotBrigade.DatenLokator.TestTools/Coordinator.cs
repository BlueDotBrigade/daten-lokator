namespace BlueDotBrigade.DatenLokator.TestsTools
{
	using System;
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	internal class Coordinator
	{
		private readonly IFileManagementStrategy _fileManagementStrategy;
		private readonly ITestNamingStrategy _testNamingStrategy;

		private bool _isIniitialized;

		public Coordinator(IFileManagementStrategy fileManagementStrategy, ITestNamingStrategy testNamingStrategy)
		{
			_fileManagementStrategy = fileManagementStrategy;
			_testNamingStrategy = testNamingStrategy;
		}

		public void Setup(string rootDirectoryPath)
		{
			_fileManagementStrategy.Setup(rootDirectoryPath);
			_isIniitialized = true;
		}

		public void Setup(string rootDirectoryPath, IDictionary<string, object> testEnvironmentSettings)
		{
			_fileManagementStrategy.Setup(rootDirectoryPath, testEnvironmentSettings);
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
					$"The {nameof(Coordinator)} has not been initialized. Hint: Call Setup() method");
			}
		}
	}
}
