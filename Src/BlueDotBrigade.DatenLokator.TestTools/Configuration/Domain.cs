namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using System.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	public sealed class Domain
	{
		private static readonly IFileManager DefaultFileManager;
		private static readonly ITestNamingStrategy DefaultTestNamingStrategy;

		internal static readonly DomainSettings Settings;

        private readonly IOsDirectory _directory;
        private readonly IOsFile _file;

		private IFileManager _fileManagerStrategy;
		private ITestNamingStrategy _testNamingStrategy;

		static Domain()
		{
			DefaultFileManager = new SimpleFileManager();
			DefaultTestNamingStrategy = new AssertActArrangeStrategy();

			Settings = new DomainSettings
			{
				DefaultFile = string.Empty,
				FileManager = new FileManager(DefaultFileManager, DefaultTestNamingStrategy),
			};
		}

		public Domain()
		{
			_directory = new OsDirectory();
			_file = new OsFile();

			_fileManagerStrategy = DefaultFileManager;
			_testNamingStrategy = DefaultTestNamingStrategy;
		}

		public static Domain Get()
        {
            return new Domain();
        }

        public Domain UsingDefaultFile(string path)
        {
            Settings.DefaultFile = path;
            return this;
        }

        public Domain UsingTestNamingConvention(ITestNamingStrategy strategy)
        {
	        _testNamingStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

        public Domain UsingFileManager(IFileManager strategy)
        {
	        _fileManagerStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

	        return this;
        }

		public void Setup()
        {
	        Settings.FileManager = new FileManager(_fileManagerStrategy, _testNamingStrategy);

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
