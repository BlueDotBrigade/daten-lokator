namespace BlueDotBrigade.DatenLokator.TestsTools.UnitTesting
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.IO;
	using System.IO.Compression;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	/// <summary>
	///     Uses "Convention Over Configuration" to simplify the management of automated test data.
	/// </summary>
	public sealed class InputPathSelector
	{
		/// <summary>
		///     Represents the application setting key that is used to explicitly define the <see cref="BaseDirectoryPath" /> path.
		/// </summary>
		internal const string BasePathKey = "InputDataBasePath";

		/// <summary>
		///     Represents the name of the sub-directory where all of the input data will be stored.
		/// </summary>
		public const string BaseDirectoryName = "Dat";

		public const string GlobalDirectory = "~Global";

		public const string CompressedFileExtension = ".Zip";
		public const string CompressedFileTempDirectory = "~ZIP";

		private readonly NameValueCollection _appSettings;

		private readonly string _executingAssemblyPath;

		private readonly IOsDirectory _directory;
		private readonly IOsFile _file;

		public InputPathSelector(IOsDirectory directory, IOsFile file, NameValueCollection appSettings,
			 string executingAssemblyPath)
		{
			if (string.IsNullOrWhiteSpace(executingAssemblyPath))
			{
				throw new ArgumentException("message", nameof(executingAssemblyPath));
			}

			_directory = directory ?? throw new ArgumentNullException(nameof(directory));
			_file = file ?? throw new ArgumentNullException(nameof(file));
			_appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
			_executingAssemblyPath = executingAssemblyPath;
		}

		public string GlobalDirectoryPath
		{
			get { return Path.Combine(this.BaseDirectoryPath, InputPathSelector.GlobalDirectory); }
		}

		// TODO: Implement more reliable way to determine the base directory path.
		// ... The executing assembly path can vary dramatically depending on a number of variables including the project's "bitness".
		// ... 32 bit output directory: bin\Debug\
		// ... 64 bit output directory: bin\x64\Debug\
		public string BaseDirectoryPath
		{
			get
			{
				var result = string.Empty;

				if (_appSettings.HasKeys())
				{
					var value = _appSettings[BasePathKey];

					// Does key exist in configuration file?
					// ... if not, then default to the assumed local directoryPath
					if (!string.IsNullOrWhiteSpace(value))
					{
						result = value;
					}
				}

				if (string.IsNullOrWhiteSpace(result))
				{
					var index = _executingAssemblyPath.LastIndexOf(@"\bin\");
					var projectDirectoryPath = _executingAssemblyPath.Substring(0, index);
					
					result = System.IO.Path.Combine(projectDirectoryPath, BaseDirectoryName);
				}

				// Ensure that there are no relative path references (i.e. return a real path)
				return System.IO.Path.GetFullPath(result);
			}
		}

		public void Setup()
		{
			var basePath = BaseDirectoryPath;

			if (string.IsNullOrWhiteSpace(basePath))
			{
				throw new InvalidOperationException(
					 "The automated test framework is unable to identify where the input files are stored.");
			}

			basePath = System.IO.Path.GetFullPath(basePath);

			if (!_directory.Exists(basePath))
			{
				throw new DirectoryNotFoundException(
					 $"The automated test framework does not have a valid directory path. Path=`{basePath}`");
			}

			Decompress(basePath);
		}

		public void Decompress(string directoryPath)
		{
			var isDecompressedArchive = CultureInfo.InvariantCulture.CompareInfo.IndexOf(
													directoryPath,
													CompressedFileTempDirectory,
													CompareOptions.IgnoreCase) >= 0;

			if (isDecompressedArchive)
			{
				// this directory holds content from a *.zip file
				// ... we can skip over it
			}
			else
			{
				var originalDirectories = _directory.GetDirectories(directoryPath);

				foreach (var file in _directory.GetFiles(directoryPath))
				{
					// ReSharper disable once PossibleNullReferenceException

					var isCompressedFile = CultureInfo.InvariantCulture.CompareInfo.IndexOf(
						                       System.IO.Path.GetExtension(file),
						                       CompressedFileExtension,
						                       CompareOptions.IgnoreCase) >= 0;

					if (isCompressedFile)
					{
						var newDirectoryName =
							System.IO.Path.GetFileNameWithoutExtension(file) + CompressedFileTempDirectory;
						// ReSharper disable once AssignNullToNotNullAttribute
						var targetDirectory =
							System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), newDirectoryName);

						if (!_directory.Exists(targetDirectory))
						{
							ZipFile.ExtractToDirectory(file, targetDirectory);
						}
					}
				}

				foreach (var directory in originalDirectories)
				{
					Decompress(directory);
				}
			}
		}

		/// <seealso cref="Setup" />
		public void Teardown()
		{
			// nothing to do
			// ... method exists for continuity
		}

		/// <summary>
		///     Determines the unit test's path relative to the project directory, and then appends the result to the provided
		///     <see cref="BaseDirectoryPath" />.
		/// </summary>
		internal string GraphOnToBaseDirectory(string unitTestPath)
		{
			if (string.IsNullOrWhiteSpace(unitTestPath))
			{
				throw new ArgumentException("Expected a valid directory path.", nameof(unitTestPath));
			}

			var index = unitTestPath.LastIndexOfPrefix(_executingAssemblyPath);
			var unitTestRelativePath = unitTestPath.Substring(index + 1);

			return System.IO.Path.Combine(BaseDirectoryPath, unitTestRelativePath);
		}

		/// <summary>
		///     Constructs a fully qualified path to the input file that will be used for the test.
		/// </summary>
		/// <remarks>
		///     <see cref="GetFilePathOrInfer" /> is not publicly exposed, because we want to encourage `Convention Over
		///     Configuration` via the provided helper methods.
		/// </remarks>
		/// <exception cref="FileNotFoundException"/>
		internal string GetFilePathOrInfer(string fileName, string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new System.ArgumentException("A file name (or unit test name) was expected.", nameof(fileName));
			}

			if (string.IsNullOrWhiteSpace(directoryPath))
			{
				throw new System.ArgumentException("A directory path (or the path to an automated test) was expected.",
					 nameof(directoryPath));
			}

			var filePath = string.Empty;

			var isSearching = true;
			var searchPaths = GetSearchPaths(fileName, directoryPath);

			for (int i = 0; i < searchPaths.Length && isSearching; i++)
			{
				try
				{
					filePath = GetFileOrInferName(fileName, searchPaths[i]);
					isSearching = false;
				}
				catch (FileNotFoundException) 
				{
					// the file is not in the search path
					// ... that's ok, lets try another directory
				}
				catch (DirectoryNotFoundException)
				{
					// the given search path does not refer to a valid directory
					// ... that's ok, lets try another directory
				}
			}

			if (string.IsNullOrWhiteSpace(filePath))
			{
				var firstFilePath = Path.Combine(directoryPath, fileName);

				var prioritizedSearchPaths = string.Empty;

				for (int i = 0; i < searchPaths.Length; i++)
				{
					prioritizedSearchPaths += $"\r\n\tPriority={i}, Path={searchPaths[i]}";
				}

				throw new FileNotFoundException(
					$@"The specified file could not be found. DirectoryPath=`{directoryPath}\`, FileName=`{fileName}`{prioritizedSearchPaths}",
					firstFilePath);
			}

			return filePath;
		}

		private string[] GetSearchPaths(string fileName, string sourceDirectory)
		{
			const string SourceCodeFileExtension = ".cs";

			var searchPaths = new List<string>();

			var isDirectoryImplied = sourceDirectory.EndsWith(SourceCodeFileExtension, StringComparison.InvariantCultureIgnoreCase);

			if (isDirectoryImplied)
			{
				var impliedDirectory = GraphOnToBaseDirectory(sourceDirectory).Replace(SourceCodeFileExtension, string.Empty);
				searchPaths.Add(impliedDirectory);
				searchPaths.Add(impliedDirectory + CompressedFileTempDirectory);

			}
			else
			{
				searchPaths.Add(sourceDirectory);
				searchPaths.Add(sourceDirectory + CompressedFileTempDirectory);
			}

            var decompressedArchive = Path.GetFileNameWithoutExtension(fileName) + CompressedFileTempDirectory;

			// try looking in the shared file cache
            searchPaths.Add(Path.Combine(this.GlobalDirectoryPath, decompressedArchive));
			searchPaths.Add(this.GlobalDirectoryPath);
            
            return searchPaths.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"/>
		/// <exception cref="DirectoryNotFoundException"/>
		private string GetFileOrInferName(string filename, string directoryPath)
		{
			var filePath = System.IO.Path.Combine(directoryPath, filename);

			// Can we find the requested file? No, then:
			// ... 1. a filename was explicitly provided and the file does not exist, or
			// ... 2. we have to infer the filename from the unit test
			if (!_file.Exists(filePath))
			{
				var unitTestName = filename.Split('_');
				if (unitTestName.Length == 3)
				{
					var scenarioName = $"{unitTestName[(int)AssertActArrangeParts.Scenario]}";
					filePath = System.IO.Path.Combine(directoryPath, scenarioName);

					// Are we looking for a file without an extension?
					if (!_file.Exists(filePath))
					{
						var matchingFiles = _directory.GetFiles(
							 directoryPath,
							 $"{scenarioName}.*",
							 System.IO.SearchOption.TopDirectoryOnly);

						if (matchingFiles.Length == 0)
						{
							throw new System.IO.FileNotFoundException(
								 $@"The implied file could not be found. DirectoryPath=`{directoryPath}\`, FileName=`{scenarioName}`",
								 filePath);
						}
						else
						{
							filePath = System.IO.Path.Combine(directoryPath, matchingFiles[0].Name);
						}
					}
				}
				else
				{
					throw new System.IO.FileNotFoundException(
						 $@"The specified file could not be found. DirectoryPath=`{directoryPath}\`, FileName=`{filename}`",
						 filePath);
				}
			}

			return filePath;
		}
	}
}