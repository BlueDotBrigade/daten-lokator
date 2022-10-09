namespace BlueDotBrigade.DatenLokator.TestsTools
{
	using System;
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;

	internal class Coordinator
	{
		private readonly IOsFile _osFile;

		private readonly IFileManagementStrategy _fileManagementStrategy;
		private readonly ITestNamingStrategy _testNamingStrategy;
		private readonly string _rootDirectoryPath;
		private readonly IDictionary<string, object> _testEnvironmentSettings;

		private readonly string _defaultFileName;

		private bool _isSetup;

		public Coordinator(
			IOsFile osFile,
			ITestNamingStrategy testNamingStrategy,
			IFileManagementStrategy fileManagementStrategy,
			IDictionary<string, object> testEnvironmentSettings,
			string defaultFileName) : this(
				osFile,
				testNamingStrategy,
				fileManagementStrategy,
				testEnvironmentSettings,
				defaultFileName,
				AssemblyHelper.DefaultInputFilePath)
		{
			// nothing to do
		}

		public Coordinator(
			IOsFile osFile,
			ITestNamingStrategy testNamingStrategy,
			IFileManagementStrategy fileManagementStrategy,
			IDictionary<string, object> testEnvironmentSettings,
			string defaultFileName,
			string rootDirectoryPath)
		{
			_osFile = osFile;
			_fileManagementStrategy = fileManagementStrategy;
			_testNamingStrategy = testNamingStrategy;
			_rootDirectoryPath = rootDirectoryPath;
			_testEnvironmentSettings = testEnvironmentSettings;
			_defaultFileName = defaultFileName ?? string.Empty;
		}

		public bool IsSetup => _isSetup;

		internal IOsFile OsFile => _osFile;

		public void Setup()
		{
			_fileManagementStrategy.Setup(_rootDirectoryPath, _testEnvironmentSettings);
			
			_isSetup = true;
		}

		public void TearDown()
		{
			_fileManagementStrategy.TearDown();
		}

		public string GetFilePath(string fileName, string sourceDirectory)
		{
			if (_isSetup)
			{
				return _fileManagementStrategy.GetFilePath(_testNamingStrategy, fileName, sourceDirectory);
			}
			else
			{
				throw new InvalidOperationException(
					$"The {nameof(Coordinator)} has not been initialized. Hint: Call Setup() method");
			}
		}

		public string GetDefaultFilePath()
		{
			return _fileManagementStrategy.GetDefaultFilePath(_defaultFileName);
		}
	}
}
