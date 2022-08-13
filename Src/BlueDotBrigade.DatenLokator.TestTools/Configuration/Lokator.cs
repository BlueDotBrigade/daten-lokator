namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	public sealed class Lokator
	{
		private static readonly IDictionary NoEnvironmentSettings = new Dictionary<string, object>();

		private static readonly IFileManagementStrategy DefaultFileManagementStrategy;
		private static readonly ITestNamingStrategy DefaultTestNamingStrategy;

		internal static readonly LokatorConfiguration Settings;

        private readonly IOsDirectory _directory;
        private readonly IOsFile _file;

		private IFileManagementStrategy _fileManagementStrategy;
		private ITestNamingStrategy _testNamingStrategy;

		private IDictionary _testEnvironmentSettings;

		static Lokator()
		{
			DefaultFileManagementStrategy = new SimpleFileManagementStrategy();
			DefaultTestNamingStrategy = new AssertActArrangeStrategy();

			Settings = new LokatorConfiguration
			{
				DefaultFile = string.Empty,
				FileManager = new FileManager(DefaultFileManagementStrategy, DefaultTestNamingStrategy),
			};
		}

		public Lokator()
		{
			_directory = new OsDirectory();
			_file = new OsFile();

			_fileManagementStrategy = DefaultFileManagementStrategy;
			_testNamingStrategy = DefaultTestNamingStrategy;

			_testEnvironmentSettings = NoEnvironmentSettings;
		}

		public static Lokator Get()
        {
            return new Lokator();
        }

        public Lokator UsingDefaultFile(string path)
        {
            Settings.DefaultFile = path;
            return this;
        }

        public Lokator UsingTestNamingStrategy(ITestNamingStrategy strategy)
        {
	        _testNamingStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

        public Lokator UsingFileManagementStrategy(IFileManagementStrategy strategy)
        {
	        _fileManagementStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

        public Lokator UsingTestContext(TestContext context)
        {
			_testEnvironmentSettings = context.Properties;
	        return this;
        }

		public void Setup()
        {
	        Settings.FileManager = new FileManager(_fileManagementStrategy, _testNamingStrategy);

			Settings.FileManager.Setup(
				_directory,
				_file,
				AssemblyHelper.ExecutingDirectory,
				_testEnvironmentSettings);
		}

		public void TearDown()
        {
	        Settings.FileManager.TearDown();
		}
	}
}
