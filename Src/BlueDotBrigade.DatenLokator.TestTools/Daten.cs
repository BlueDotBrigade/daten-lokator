﻿namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Text.Json;
	using BlueDotBrigade.DatenLokator.TestTools.Configuration;

	public class Daten
	{
		private readonly string _callingMethodName;
		private readonly string _callingClassPath;
		private const string DoNotSet = "";

		private readonly Coordinator _coordinator;

		/// <summary>
		/// Retrieves data for the test that is currently executing.
		/// </summary>
		/// <param name="callingMethodName">Do not provide a value </param>
		/// <param name="callingClassPath">Do not provide a value.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public Daten(
			[CallerMemberName] string callingMethodName = DoNotSet,
			[CallerFilePath] string callingClassPath = DoNotSet)
			: this(
				Lokator.Get().Coordinator,
				callingMethodName, 
				callingClassPath)
		{
			// nothing to do
		}

		internal Daten(
			Coordinator coordinator,
			[CallerMemberName] string callingMethodName = DoNotSet,
			[CallerFilePath] string callingClassPath = DoNotSet)
		{
			_callingMethodName = callingMethodName ?? throw new ArgumentNullException(nameof(callingMethodName));
			_callingClassPath = callingClassPath ?? throw new ArgumentNullException(nameof(callingClassPath));

			_coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));

			if (!_coordinator.IsSetup)
			{
				throw new InvalidOperationException("The test environment has not yet been initialized. Hint: Call Lokator.Setup().");
			}
		}

		private void ThrowIfFileMissing(string path)
		{
			if (!_coordinator.OsFile.Exists(path))
			{
				var sourceFile = System.IO.Path.GetFileName(path);
				var directoryPath = System.IO.Path.GetDirectoryName(path) + @"\";
				throw new System.IO.FileNotFoundException(
					$@"Unable to find the requested input file. Directory=`{directoryPath}`, File=`{sourceFile}`",
					path);
			}

			System.Console.WriteLine($"Source data has been selected. FileName=`{System.IO.Path.GetFileName(path)}`");
		}

		private string GetGlobalDefaultPath(From fromSource)
		{
			var defaultFilePath = string.Empty;

			switch (fromSource)
			{
				case From.GlobalDefault:
					defaultFilePath = _coordinator.GetDefaultFilePath();
					break;

				default:
					throw new ArgumentOutOfRangeException(
						nameof(fromSource),
						$"This method expects the parameter to always be: {nameof(From.GlobalDefault)}");
			}

			return defaultFilePath;
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns the source file as a fully qualified path.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsFilePath()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return sourceFilePath;
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns the source file as a fully qualified path.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsFilePath(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return sourceFilePath;
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="fromSource">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns the source file as a fully qualified path.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public string AsFilePath(From fromSource)
		{
			var sourceFilePath = GetGlobalDefaultPath(fromSource);

			ThrowIfFileMissing(sourceFilePath);

			return sourceFilePath;
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns the source file as a .NET <see langword="string"/>.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsString()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.ReadAllText(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns the source file as a .NET <see langword="string"/>.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public string AsString(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.ReadAllText(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="fromSource">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns the source file as a .NET <see langword="string"/>.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public string AsString(From fromSource)
		{
			var sourceFilePath = GetGlobalDefaultPath(fromSource);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.ReadAllText(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns a <see langword="byte"/> array which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public byte[] AsBytes()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.ReadAllBytes(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns a <see langword="byte"/> array which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public byte[] AsBytes(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.ReadAllBytes(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="fromSource">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns a <see langword="byte"/> array which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public byte[] AsBytes(From fromSource)
		{
			var sourceFilePath = GetGlobalDefaultPath(fromSource);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.ReadAllBytes(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.Stream"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.Stream AsStream()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.OpenRead(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.Stream"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.Stream AsStream(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.OpenRead(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="fromSource">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns a <see cref="System.IO.Stream"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public System.IO.Stream AsStream(From fromSource)
		{
			var sourceFilePath = GetGlobalDefaultPath(fromSource);

			ThrowIfFileMissing(sourceFilePath);

			return _coordinator.OsFile.OpenRead(sourceFilePath);
		}

		/// <summary>
		/// Retrieves the data that is appropriate for the test that is currently executing.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.StreamReader"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.StreamReader AsStreamReader()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return new System.IO.StreamReader(_coordinator.OsFile.OpenRead(sourceFilePath));
		}

		/// <summary>
		/// Retrieves the data that is stored within the given <paramref name="fileName"/>.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="System.IO.StreamReader"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		public System.IO.StreamReader AsStreamReader(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			return new System.IO.StreamReader(_coordinator.OsFile.OpenRead(sourceFilePath));
		}

		/// <summary>
		/// Retrieves the data that was registered with <see cref="Lokator"/>.
		/// </summary>
		/// <param name="fromSource">Determines which registered file to retrieve.</param>
		/// <returns>
		/// Returns a <see cref="System.IO.StreamReader"/> which encapsulates source file as a sequence of bytes.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"/>
		public System.IO.StreamReader AsStreamReader(From fromSource)
		{
			var sourceFilePath = GetGlobalDefaultPath(fromSource);

			ThrowIfFileMissing(sourceFilePath);

			return new System.IO.StreamReader(_coordinator.OsFile.OpenRead(sourceFilePath));
		}

		/// <summary>
		/// Finds and reads the JSON file for the currently running test, and deserializes it into <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">Target type for the JSON content.</typeparam>
		/// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
		/// <remarks>
		/// The file is looked up in this order:
		/// 1. the test’s specific directory  
		/// 2. a compressed file matching that directory  
		/// 3. a shared global directory  
		/// </remarks>
		public T FromJson<T>()
		{
			var sourceFilePath = _coordinator.GetFilePath(_callingMethodName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			var jsonContent = _coordinator.OsFile.ReadAllText(sourceFilePath);
			return JsonSerializer.Deserialize<T>(jsonContent);
		}

		/// <summary>
		/// Finds and reads the JSON file for the currently running test, and deserializes it into <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">Target type for the JSON content.</typeparam>
		/// <param name="fileName">The name of the JSON file to read.</param>
		/// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
		/// <remarks>
		/// The file is looked up in this order:
		/// 1. the test’s specific directory  
		/// 2. a compressed file matching that directory  
		/// 3. a shared global directory  
		/// </remarks>
		public T FromJson<T>(string fileName)
		{
			var sourceFilePath = _coordinator.GetFilePath(fileName, _callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

			var jsonContent = _coordinator.OsFile.ReadAllText(sourceFilePath);
			return JsonSerializer.Deserialize<T>(jsonContent);
		}


		/// <summary>
		/// Finds and reads the JSON file for the currently running test, and deserializes it into <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">Target type for the JSON content.</typeparam>
		/// <param name="fromSource">Forces the global default file to be returned.</param>
		/// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
		/// <remarks>
		/// The file is looked up in this order:
		/// 1. the test’s specific directory  
		/// 2. a compressed file matching that directory  
		/// 3. a shared global directory  
		/// </remarks>
		public T FromJson<T>(From fromSource)
		{
			var sourceFilePath = GetGlobalDefaultPath(fromSource);

			ThrowIfFileMissing(sourceFilePath);

			var jsonContent = _coordinator.OsFile.ReadAllText(sourceFilePath);
			return JsonSerializer.Deserialize<T>(jsonContent);
		}
	}
}