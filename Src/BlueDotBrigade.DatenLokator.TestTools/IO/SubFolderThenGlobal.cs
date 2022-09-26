﻿namespace BlueDotBrigade.DatenLokator.TestsTools.IO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.IO.Compression;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	internal class SubFolderThenGlobal : IFileManagementStrategy
	{
		/// <summary>
		/// Represents the name of the directory that holds input data that is shared by several automated tests.
		/// </summary>
		/// <remarks>
		/// This directory contains:
		/// 1. very large input files
		/// 2. data that is shared by a large numnber of tests
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

		public string GlobalDirectoryPath => Path.Combine(this.RootDirectoryPath, GlobalDirectoryName);
		
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
		}
		public void Setup(string rootDirectoryPath, IDictionary<string, object> testEnvironmentProperties)
		{
			var settings = testEnvironmentProperties ?? throw new ArgumentNullException(nameof(testEnvironmentProperties));

			Setup();
		}

		public void Setup()
		{
			Decompress(_rootDirectoryPath);
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

		public string GetFileOrInferName(ITestNamingStrategy testNamingStrategy, string fileNameOrHint, string sourceDirectory)
		{
			var sourceFilePath = Path.Combine(sourceDirectory, fileNameOrHint);

			if (_file.Exists(sourceFilePath))
			{
				// A real file name was provided. No further action requr
			}
			else
			{
				if (testNamingStrategy.TryGetFileName(fileNameOrHint, out var impliedFileName))
				{
					sourceFilePath = Path.Combine(sourceDirectory, impliedFileName);

					if (_file.Exists(sourceFilePath))
					{
						// Naming strategy helped us find the source file
						// ... which does not have a file extension.
					}
					else
					{
						var matchingFiles = _directory.GetFiles(
							sourceDirectory,
							$"{impliedFileName}.*",
							SearchOption.TopDirectoryOnly);

						switch (matchingFiles.Length)
						{
							case 0:
								throw new FileNotFoundException(
									$@"The implied file could not be found. DirectoryPath=`{sourceDirectory}\`, ImpliedName=`{impliedFileName}`",
									sourceFilePath);

							case 1:
								sourceFilePath = Path.Combine(sourceDirectory, matchingFiles[0].Name);
								break;

							default:
								throw new FileNotFoundException(
									$@"The implied file could not be found because multiple files have the same name. DirectoryPath=`{sourceDirectory}\`, ImpliedName=`{impliedFileName}.*`",
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
					$@"The specified file could not be found. DirectoryPath=`{sourceDirectory}\`, FileName=`{fileNameOrHint}`{prioritizedSearchPaths}",
					firstFilePath);
			}

			return filePath;
		}
	}
}