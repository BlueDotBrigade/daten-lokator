namespace BlueDotBrigade.DatenLokator.TestsTools
{
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	public class Daten
	{
		private const string DoNotSet = "";

		private readonly IOsDirectory _osDirectory;
		private readonly IOsFile _osFile;

		private readonly Coordinator _coordinator;

		public Daten()
		{
			var lokator = Lokator.Get();

			_osDirectory = lokator.OsDirectory;
			_osFile = lokator.OsFile;

			_coordinator = lokator.Coordinator;
		}

		internal Daten(Lokator lokator)
		{
			_osDirectory = lokator.OsDirectory;
			_osFile = lokator.OsFile;

			_coordinator = lokator.Coordinator;
		}

		private  void ThrowIfFileMissing(string path)
        {
            if (!_osFile.Exists(path))
            {
                var sourceFile = System.IO.Path.GetFileName(path);
                var directoryPath = System.IO.Path.GetDirectoryName(path) + @"\";
                throw new System.IO.FileNotFoundException(
                    $@"Unable to find the requested input file. Directory=`{directoryPath}`, File=`{sourceFile}`",
                    path);
            }

            System.Console.WriteLine($"Input data has been selected. SourceFileName=`{System.IO.Path.GetFileName(path)}`");
        }

		/// <summary>
		///     Retrieves the path to a file based on the provided parameters.
		/// </summary>
		/// <param name="callingMethodName">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter to the name of the calling method.
		///     The <see cref="ITestNamingStrategy"/> will then select an input file based on the current test.
		/// </param>
		/// <param name="callingClassPath">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter the calling class' file path.
		///     The <see cref="IFileManagementStrategy"/> will then build a directory path based on the current test.
		/// </param>
		/// <returns>A fully qualified file path.</returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026")] // default is required by `CallerFilePath`
        public string AsFilePath(
	        [CallerMemberName] string callingMethodName = DoNotSet,
            [CallerFilePath] string callingClassPath = DoNotSet)
        {
	        var sourceFilePath = _coordinator.GetFilePath(callingMethodName, callingClassPath);

            ThrowIfFileMissing(sourceFilePath);

            return sourceFilePath;
        }

		/// <summary>
		///     Retrieves the content of a text file.
		/// </summary>
		/// <param name="callingMethodName">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter to the name of the calling method.
		///     The <see cref="ITestNamingStrategy"/> will then select an input file based on the current test.
		/// </param>
		/// <param name="callingClassPath">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter the calling class' file path.
		///     The <see cref="IFileManagementStrategy"/> will then build a directory path based on the current test.
		/// </param>
		/// <returns>A fully qualified file path.</returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026")] // default is required by `CallerFilePath`
        public string AsString(
	        [CallerMemberName] string callingMethodName = DoNotSet,
            [CallerFilePath] string callingClassPath = DoNotSet)
        {
	        var sourceFilePath = _coordinator.GetFilePath(callingMethodName, callingClassPath);

            ThrowIfFileMissing(sourceFilePath);

            return _osFile.ReadAllText(sourceFilePath);
        }

		/// <summary>
		///     Retrieves the content of a text file.
		/// </summary>
		/// <param name="callingMethodName">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter to the name of the calling method.
		///     The <see cref="ITestNamingStrategy"/> will then select an input file based on the current test.
		/// </param>
		/// <param name="callingClassPath">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter the calling class' file path.
		///     The <see cref="IFileManagementStrategy"/> will then build a directory path based on the current test.
		/// </param>
		/// <returns>A fully qualified file path.</returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026")] // default is required by `CallerFilePath`
        public System.IO.Stream AsStream(
	        [CallerMemberName] string callingMethodName = DoNotSet,
            [CallerFilePath] string callingClassPath = DoNotSet)
        {
	        var sourceFilePath = _coordinator.GetFilePath(callingMethodName, callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

            return _osFile.OpenRead(sourceFilePath);
        }

		/// <summary>
		///     Retrieves the content of a text file.
		/// </summary>
		/// <param name="callingMethodName">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter to the name of the calling method.
		///     The <see cref="ITestNamingStrategy"/> will then select an input file based on the current test.
		/// </param>
		/// <param name="callingClassPath">
		///     Do not provide a value.
		///     The .NET runtime will automatically set this parameter the calling class' file path.
		///     The <see cref="IFileManagementStrategy"/> will then build a directory path based on the current test.
		/// </param>
		/// <returns>A fully qualified file path.</returns>
		/// <remarks>
		/// Directory search order:
		/// 1. the given directory
		/// 2. a compressed file that is similar to the given directory
		/// 3. the global directory for shared files
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026")] // default is required by `CallerFilePath`
        public System.IO.StreamReader AsStreamReader(
	        [CallerMemberName] string callingMethodName = DoNotSet,
            [CallerFilePath] string callingClassPath = DoNotSet)
        {
	        var sourceFilePath = _coordinator.GetFilePath(callingMethodName, callingClassPath);

			ThrowIfFileMissing(sourceFilePath);

            return new System.IO.StreamReader(_osFile.OpenRead(sourceFilePath));
        }
    }
}
