namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.IO;
	using System.IO.Compression;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;

	internal class SimpleFileManagementStrategy : IFileManagementStrategy
	{
		/// <summary>
		/// Represents the application setting key that is used to explicitly define the <see cref="BaseDirectoryPath" /> path.
		/// </summary>
		internal const string BasePathKey = "InputDataBasePath";

		/// <summary>
		/// Represents the name of the directory that holds input data that is shared by several automated tests.
		/// </summary>
		/// <remarks>
		/// This directory contains:
		/// 1. very large input files
		/// 2. data that is shared by a large numnber of tests
		/// </remarks>
		public const string SharedDataDirectory = "~Global";

		/// <summary>
		/// Represents the name of the root directory that all input data is stored within.
		/// </summary>
		/// <remarks>
		/// By default, this directory is assumed to be in the same folder as `.csproj`.
		/// </remarks>
		public const string BaseDirectory = "Dat";

		public const string CompressedFileExtension = ".Zip";
		public const string CompressedFileTempDirectory = "~ZIP";

		private NameValueCollection _appSettings;

		private string _executingAssemblyPath;

		private IOsDirectory _directory;
		private IOsFile _file;


		public string SharedDirectoryPath
		{
			get { return Path.Combine(this.BaseDirectoryPath, SharedDataDirectory); }
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

					result = Path.Combine(projectDirectoryPath, BaseDirectory);
				}

				// Ensure that there are no relative path references (i.e. return a real path)
				return Path.GetFullPath(result);
			}
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
						Path.GetExtension(file),
						CompressedFileExtension,
						CompareOptions.IgnoreCase) >= 0;

					if (isCompressedFile)
					{
						var newDirectoryName =
							Path.GetFileNameWithoutExtension(file) + CompressedFileTempDirectory;
						// ReSharper disable once AssignNullToNotNullAttribute
						var targetDirectory =
							Path.Combine(Path.GetDirectoryName(file), newDirectoryName);

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

		public void Setup(
			IOsDirectory directory,
			IOsFile file,
			string executingAssemblyPath,
			NameValueCollection applicationSettings)
		{
			if (string.IsNullOrWhiteSpace(executingAssemblyPath))
			{
				throw new ArgumentException("message", nameof(executingAssemblyPath));
			}

			_directory = directory ?? throw new ArgumentNullException(nameof(directory));
			_file = file ?? throw new ArgumentNullException(nameof(file));
			_appSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
			_executingAssemblyPath = executingAssemblyPath;

			var basePath = BaseDirectoryPath;

			if (string.IsNullOrWhiteSpace(basePath))
			{
				throw new InvalidOperationException(
					"The automated test framework is unable to identify where the input files are stored.");
			}

			basePath = Path.GetFullPath(basePath);

			if (!_directory.Exists(basePath))
			{
				throw new DirectoryNotFoundException(
					$"The automated test framework does not have a valid directory path. Path=`{basePath}`");
			}

			Decompress(basePath);
		}

		public void TearDown()
		{
			// nothing to do
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
			searchPaths.Add(Path.Combine(this.SharedDirectoryPath, decompressedArchive));
			searchPaths.Add(this.SharedDirectoryPath);

			return searchPaths.ToArray();
		}

		/// <summary>
		///  Determines the unit test's path relative to the project directory, and then appends the result to the provided <see cref="BaseDirectoryPath" />.
		/// </summary>
		internal string GraphOnToBaseDirectory(string unitTestPath)
		{
			if (string.IsNullOrWhiteSpace(unitTestPath))
			{
				throw new ArgumentException("Expected a valid directory path.", nameof(unitTestPath));
			}

			var index = unitTestPath.LastIndexOfPrefix(_executingAssemblyPath);
			var unitTestRelativePath = unitTestPath.Substring(index + 1);

			return System.IO.Path.Combine(this.BaseDirectoryPath, unitTestRelativePath);
		}

		public string GetFileOrInferName(ITestNamingStrategy testNamingStrategy, string fileNameOrHint, string sourceDirectory)
		{
			var filePath = Path.Combine(sourceDirectory, fileNameOrHint);

			// Can we find the requested file? No, then:
			// ... 1. we have to infer the filename from the test's name, or
			// ... 2. the specified file simply does not exist
			if (!_file.Exists(filePath))
			{
				if (testNamingStrategy.TryGetFileName(fileNameOrHint, out var fileName))
				{
					filePath = Path.Combine(sourceDirectory, fileName);

					// Are we looking for a file without an extension?
					if (!_file.Exists(filePath))
					{
						var matchingFiles = _directory.GetFiles(
							sourceDirectory,
							$"{fileName}.*",
							SearchOption.TopDirectoryOnly);

						if (matchingFiles.Length == 0)
						{
							throw new FileNotFoundException(
								$@"The implied file could not be found. DirectoryPath=`{sourceDirectory}\`, FileName=`{fileName}`",
								filePath);
						}
						else
						{
							filePath = Path.Combine(sourceDirectory, matchingFiles[0].Name);
						}
					}
				}
				else
				{
					throw new FileNotFoundException(
						$@"The specified file could not be found. DirectoryPath=`{sourceDirectory}\`, FileName=`{fileNameOrHint}`",
						filePath);
				}
			}

			return filePath;
		}

		public string GetFilePath(ITestNamingStrategy testNamingStrategy, string fileNameOrHint, string sourceDirectory)
		{
			if (string.IsNullOrWhiteSpace(fileNameOrHint))
			{
				throw new System.ArgumentException("A file name (or unit test name) was expected.",
					nameof(fileNameOrHint));
			}

			if (string.IsNullOrWhiteSpace(sourceDirectory))
			{
				throw new System.ArgumentException("A directory path (or the path to an automated test) was expected.",
					nameof(sourceDirectory));
			}

			var filePath = string.Empty;

			var isStillSearching = true;
			var searchPaths = GetSearchPaths(fileNameOrHint, sourceDirectory);

			for (int i = 0; i < searchPaths.Length && isStillSearching; i++)
			{
				try
				{
					filePath = GetFileOrInferName(testNamingStrategy, fileNameOrHint, searchPaths[i]);
					isStillSearching = false;
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
				var firstFilePath = Path.Combine(sourceDirectory, fileNameOrHint);

				var prioritizedSearchPaths = string.Empty;

				for (int i = 0; i < searchPaths.Length; i++)
				{
					prioritizedSearchPaths += $"\r\n\tPriority={i}, Path={searchPaths[i]}";
				}

				throw new FileNotFoundException(
					$@"The specified file could not be found. DirectoryPath=`{sourceDirectory}\`, FileName=`{fileNameOrHint}`{prioritizedSearchPaths}",
					firstFilePath);
			}

			return filePath;
		}
	}
}
