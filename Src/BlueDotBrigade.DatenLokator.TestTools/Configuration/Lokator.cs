namespace BlueDotBrigade.DatenLokator.TestTools.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestTools.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestTools.Reflection;
	using BlueDotBrigade.DatenLokator.TestTools.Uri;

	public sealed class Lokator
	{
		private static readonly Lokator SharedInstance = new Lokator();

		private readonly LokatorConfiguration _configuration;

		private Coordinator _coordinator = null;
		private readonly IOsDirectory _osDirectory;
		private readonly IOsFile _osFile;

		private bool _isSetup = false;

		internal Lokator() : this(new OsDirectory(), new OsFile())
		{
			// nothing to do
		}

		internal Lokator(IOsDirectory osOsDirectory, IOsFile osOsFile)
		{
			_osDirectory = osOsDirectory;
			_osFile = osOsFile;

			ITestNamingStrategy testNamingStrategy = new MemberCaseResultNamingStrategy();

			IFileManagementStrategy fileManagementStrategy = new SubFolderThenGlobal(
				_osDirectory,
				_osFile);

			_configuration = new LokatorConfiguration(testNamingStrategy, fileManagementStrategy);

			_coordinator = new Coordinator(
				_osFile,
				_configuration.TestNamingStrategy,
				_configuration.FileManagementStrategy,
				_configuration.TestEnvironmentProperties,
				_configuration.DefaultFileName);
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

		public Lokator UsingDefaultFileName(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentNullException(nameof(fileName),"A valid filename was expected.");
			}

			_configuration.DefaultFileName = fileName;
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

		/// <summary>
		/// Specifies the configuration parameters that should be used by the test environment.
		/// </summary>
		/// <param name="properties">A collection of configuration parameters.</param>
		/// <remarks>
		/// By default, DatenLokator assumes that the intput data is stored within the project's directory.
		///
		/// A new data directory can be specfied by adding the following key to the <see cref="TestContext"/>:
		/// <example>
		/// context.Properties["DatenLokatorRootPath"] = @"c:\New\Path\Goes\Here";
		/// </example>
		/// </remarks>
		/// <seealso href="https://learn.microsoft.com/en-us/previous-versions/ms404699(v=vs.90)">MSDN: Using the TestContext</seealso>
		/// <seealso href="https://blog.adilakhter.com/2008/05/04/more-on-unit-testing-testcontext/">More on Unit Testing: TestContext</seealso>
		public Lokator UsingTestContext(IDictionary properties)
		{
			_configuration.TestEnvironmentProperties = properties as IDictionary<string, object>;

			return this;
		}

		private string GetRootDirectoryPath()
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
			var rootDirectoryPath = GetRootDirectoryPath();

			if (string.IsNullOrEmpty(rootDirectoryPath))
			{
				rootDirectoryPath = AssemblyHelper.DefaultDatenDirectoryPath;
			}

			_coordinator = new Coordinator(
				_osFile,
				_configuration.TestNamingStrategy,
				_configuration.FileManagementStrategy,
				_configuration.TestEnvironmentProperties,
				_configuration.DefaultFileName,
				rootDirectoryPath);

			_coordinator.Setup();

			_isSetup = true;

			return this;
		}

		public Lokator TearDown()
		{
			_configuration.FileManagementStrategy.TearDown();

			return this;
		}

		/// <summary>
		/// Registers a URI to serve static content via an in-process HTTP listener.
		/// </summary>
		/// <param name="uri">The URI to register (must be absolute HTTP/HTTPS).</param>
		/// <param name="content">The content to serve (string, byte[], Stream, or any serializable object).</param>
		/// <param name="contentType">Optional content type. If not provided, will be inferred from content.</param>
		/// <returns>An IDisposable that stops the listener when disposed.</returns>
		/// <exception cref="ArgumentNullException">If uri or content is null.</exception>
		/// <exception cref="ArgumentException">If uri is not absolute or not HTTP/HTTPS.</exception>
		/// <exception cref="InvalidOperationException">If the HTTP listener cannot be started.</exception>
		public static IDisposable Register(System.Uri uri, object content, string contentType = null)
		{
			return new UrlRegistration(uri, content, contentType);
		}

		/// <summary>
		/// Registers a URI to serve content resolved by a Daten instance.
		/// </summary>
		/// <param name="uri">The URI to register (must be absolute HTTP/HTTPS).</param>
		/// <param name="daten">The Daten instance to resolve content from.</param>
		/// <param name="contentType">Optional content type. If not provided, will be inferred from content.</param>
		/// <returns>An IDisposable that stops the listener when disposed.</returns>
		/// <exception cref="ArgumentNullException">If uri or daten is null.</exception>
		/// <exception cref="ArgumentException">If uri is not absolute or not HTTP/HTTPS.</exception>
		/// <exception cref="InvalidOperationException">If the HTTP listener cannot be started.</exception>
		public static IDisposable Register(System.Uri uri, Daten daten, string contentType = null)
		{
			if (daten == null)
			{
				throw new ArgumentNullException(nameof(daten));
			}

			// Get the content from Daten
			var content = daten.AsString();
			
			return new UrlRegistration(uri, content, contentType);
		}
	}
}