namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using System.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	public sealed class Lokator
	{
		private static readonly IFileManagementStrategy DefaultFileManagementStrategy;
		private static readonly ITestNamingStrategy DefaultTestNamingStrategy;

		internal static readonly LokatorSettings Settings;

        private readonly IOsDirectory _directory;
        private readonly IOsFile _file;

		private IFileManagementStrategy _fileManagementStrategy;
		private ITestNamingStrategy _testNamingStrategy;

		static Lokator()
		{
			DefaultFileManagementStrategy = new SimpleFileManagementStrategy();
			DefaultTestNamingStrategy = new AssertActArrangeStrategy();

			Settings = new LokatorSettings
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

        public Lokator UsingTestNamingConvention(ITestNamingStrategy strategy)
        {
	        _testNamingStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

        public Lokator UsingFileManager(IFileManagementStrategy strategy)
        {
	        _fileManagementStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

		public void Setup()
        {
	        Settings.FileManager = new FileManager(_fileManagementStrategy, _testNamingStrategy);

			Settings.FileManager.Setup(
				_directory, 
				_file, 
				ConfigurationManager.AppSettings,
				AssemblyHelper.ExecutingDirectory);
        }

		public void TearDown()
        {
	        Settings.FileManager.TearDown();
		}
    }
}
