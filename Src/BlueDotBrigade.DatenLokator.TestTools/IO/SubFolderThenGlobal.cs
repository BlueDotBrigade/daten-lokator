namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	internal class SubFolderThenGlobal : IFileManagementStrategy
	{
		/// <summary>
		/// Represents the name of the directory where all of the shared input data is stored.
		/// </summary>
		/// <remarks>
		/// Typically this directory contains:
		/// 1. very large input files
		/// 2. data that is shared by several tests
		/// </remarks>
		public const string GlobalDirectoryName = "~Global";

		public const string CompressedFileExtension = ".Zip";
		public const string CompressedFileTempDirectory = "~ZIP";

		private readonly IOsDirectory _directory;
		private readonly IOsFile _file;

		private string _rootDirectoryPath;

		public SubFolderThenGlobal(IOsDirectory directory, IOsFile file)
		{
			_directory = directory ?? throw new ArgumentNullException(nameof(directory));
			_file = file ?? throw new ArgumentNullException(nameof(file));

			_rootDirectoryPath = string.Empty;
		}

		/// <summary>
		/// Path refers to the directory that holds data that is shared by multiple tests.
		/// </summary>
		public string GlobalDirectoryPath => Path.Combine(this.RootDirectoryPath, GlobalDirectoryName);

		/// <summary>
		/// Path refers to the parent directory where all of the input data is stored.
		/// </summary>
		public string RootDirectoryPath => _rootDirectoryPath;

		private void Decompress(string directoryPath)
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

		public void Setup(string rootDirectoryPath)
		{
			if (_directory.Exists(rootDirectoryPath))
			{
				_rootDirectoryPath = rootDirectoryPath;
			}
			else
			{
				throw new ArgumentException(
					$"The provided root directory does not exist. Path={rootDirectoryPath}",
					nameof(rootDirectoryPath));
			}

			Decompress(_rootDirectoryPath);
		}

		public void Setup(string rootDirectoryPath, IDictionary<string, object> testEnvironmentProperties)
		{
			// The `testEnvironmentProperties` parameter is ignored because
			// ... this class has no need for additional configuration values

			Setup(rootDirectoryPath);
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
				var impliedDirectory = AppendToRootDirectory(sourceDirectory).Replace(SourceCodeFileExtension, string.Empty);
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
		///  Determines the unit test's path relative to the project directory, and then appends the result to the provided <see cref="RootDirectoryPath" />.
		/// </summary>
		internal string AppendToRootDirectory(string subDirectoryPath)
		{
			if (string.IsNullOrWhiteSpace(subDirectoryPath))
			{
				throw new ArgumentException("Expected a valid directory path.", nameof(subDirectoryPath));
			}

			var index = subDirectoryPath.LastIndexOfPrefix(_rootDirectoryPath);
			var subDirectoryRelativePath = subDirectoryPath.Substring(index + 1);

			return System.IO.Path.Combine(this.RootDirectoryPath, subDirectoryRelativePath);
		}

		private string GetFileOrInferName(ITestNamingStrategy testNamingStrategy, string fileNameOrHint, string sourceDirectory)
		{
			var sourceFilePath = Path.Combine(sourceDirectory, fileNameOrHint);

			if (_file.Exists(sourceFilePath))
			{
				// A real file name was provided. No further action required.
			}
			else
			{
				// Try to find a file that matches the current test case name.
				if (testNamingStrategy.TryGetFileName(fileNameOrHint, out var testCaseName))
				{
					sourceFilePath = Path.Combine(sourceDirectory, testCaseName);

					if (_file.Exists(sourceFilePath))
					{
						// Naming strategy helped us find an exact match.
						// ... In this case, the file name does not include an extension.
					}
					else
					{
						var matchingFileNames = _directory.GetFiles(
							sourceDirectory,
							$"{testCaseName}.*",
							SearchOption.TopDirectoryOnly);

						switch (matchingFileNames.Count())
						{
							case 0:
								throw new FileNotFoundException(
									$@"A file for this test case could not be found. DirectoryPath=`{sourceDirectory}\`, ImpliedName=`{testCaseName}`",
									sourceFilePath);

							case 1:
								sourceFilePath = Path.Combine(sourceDirectory, matchingFileNames[0].Name);
								break;

							default:
								throw new FileNotFoundException(
									$@"Multiple files matching the test case have been found - too many results. DirectoryPath=`{sourceDirectory}\`, ImpliedName=`{testCaseName}.*`",
									sourceFilePath);
						}
					}
				}
				else
				{
					throw new FileNotFoundException(
						$@"Unable to determine the source file name. DirectoryPath=`{sourceDirectory}\`, FileNameOrHint=`{fileNameOrHint}`",
						sourceFilePath);
				}
			}

			return sourceFilePath;
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
					$@"The specified file could not be found. FileNameOrHint=`{fileNameOrHint}`{prioritizedSearchPaths}",
					firstFilePath);
			}

			return filePath;
		}
	}
}
