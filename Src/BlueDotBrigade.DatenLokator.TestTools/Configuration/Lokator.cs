namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	public sealed class Lokator
	{
		private static readonly Lokator SharedInstance = new Lokator();

		private readonly LokatorConfiguration _configuration;

		private Coordinator _coordinator = null;
		private readonly IOsDirectory _osDirectory;
		private readonly IOsFile _osFile;

		internal Lokator()
		:this(new OsDirectory(), new OsFile())
		{
			// nothing to do
		}

		internal Lokator(IOsDirectory osOsDirectory, IOsFile osOsFile)
		{
			_osDirectory = osOsDirectory;
			_osFile = osOsFile;

			ITestNamingStrategy testNamingStrategy = new AssertActArrange();

			IFileManagementStrategy fileManagementStrategy = new SubFolderThenGlobal(
				_osDirectory, 
				_osFile);

			_configuration = new LokatorConfiguration(testNamingStrategy, fileManagementStrategy);

			_coordinator = new Coordinator(
				_configuration.FileManagementStrategy,
				_configuration.TestNamingStrategy);
		}

		public IOsDirectory OsDirectory => _osDirectory;

		public IOsFile OsFile => _osFile;

		internal Coordinator Coordinator
		{
			get
			{
				if (_coordinator == null)
				{
					throw new InvalidOperationException("The library must first be initialized using: Lokator.Setup()");
				}
				else
				{
					return _coordinator;
				}
			}
		}

		public static Lokator Get()
		{
			return SharedInstance;
		}

        public Lokator UsingDefaultFile(string path)
        {
            _configuration.DefaultFilePath = path;
            return this;
        }

        public Lokator UsingTestNamingStrategy(ITestNamingStrategy strategy)
        {
	        _configuration.TestNamingStrategy = strategy;
	        return this;
        }

        public Lokator UsingFileManagementStrategy(IFileManagementStrategy strategy)
        {
	        _configuration.FileManagementStrategy = strategy;
	        return this;
        }

		public Lokator UsingTestContext(IDictionary properties)
		{
			_configuration.TestEnvironmentProperties = properties as IDictionary<string, object>;

			return this;
		}

		///// <summary>
		///// Sets up the DatenLokator environment based on the parameters defined within MS Test&apos;s <see cref="TestContext"/>.
		///// </summary>
		///// <param name="properties">Represents the <see cref="TestContext"/> properties.
		///// <example>
		///// lokator.UsingTestContext(testContext.Properties as IDictionary&lt;string, object>);
		///// </example>
		///// </param>
		//public Lokator UsingTestContext(IDictionary<string, object> properties)
		//{
		//	_configuration.TestEnvironmentProperties = properties;

		//	return this;
  //      }

		private string GetProjectDirectoryPath()
		{
			var index = AssemblyHelper.ExecutingDirectory.LastIndexOf(@"\bin\");
			var projectDirectoryPath = AssemblyHelper.ExecutingDirectory.Substring(0, index);

			return Path.Combine(projectDirectoryPath, LokatorConfiguration.RootDirectoryName);
		}

		private string GetRootPathFromTestContext()
		{
			var result = string.Empty;

			IDictionary<string, object> settings = _configuration.TestEnvironmentProperties;

			if (settings.ContainsKey(LokatorConfiguration.RootDirectoryKey))
			{
				var rootDirectoryPath = settings[LokatorConfiguration.RootDirectoryKey]?.ToString();

				if (_osDirectory.Exists(rootDirectoryPath))
				{
					result = rootDirectoryPath;
				}
				else
				{
					throw new ArgumentException($"TestContext collection was expected to include a valid root diretory path. Key={LokatorConfiguration.RootDirectoryKey}");
				}
			}
			return result;
		}

		public Lokator Setup()
        {
	        _coordinator = new Coordinator(
		        _configuration.FileManagementStrategy,
		        _configuration.TestNamingStrategy);

	        var rootDirectoryPath = GetRootPathFromTestContext();

	        if (string.IsNullOrEmpty(rootDirectoryPath))
	        {
		        rootDirectoryPath = GetProjectDirectoryPath();
	        }

	        _coordinator.Setup(rootDirectoryPath);

	        return this;
        }

		public Lokator TearDown()
        {
	        _configuration.FileManagementStrategy.TearDown();

	        return this;
		}
	}
}
