namespace BlueDotBrigade.DatenLokator.TestsTools
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;

	public class Daten
	{
		/// <summary>
		/// Using the default value of <see cref="string.Empty"/> will result in DatenLokator using the <see cref="ITestNamingStrategy"/>
		/// to determine the source file name.
		/// </summary>
		private const string UseNamingStrategy = "";

		/// <summary>
		/// Using the default value of <see cref="string.Empty"/> will result in DatenLokator receiving the calling method's path at runtime.
		/// This path will then be added to the list of search directories.
		/// </summary>
		private const string UseCallersFilePath = "";

		private readonly IOsDirectory _osDirectory;
		private readonly IOsFile _osFile;

		private readonly Coordinator _coordinator;

		public Daten()
		{
			Lokator lokator = Lokator.Get();

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

		/// <summary>
		/// Initializes an instance of the <see cref="Daten"/> class, to be used by automated testing.
		/// </summary>
		[Obsolete("Remove this constructor")]
		internal Daten(Coordinator coordinator)
		{
			_coordinator = coordinator;
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
        /// <param name="fileNameOrHint">
        ///     Explicitly specifies the file to open(filename + extension).
        ///     When omitted, the file name will be implied from the calling method's name.
        /// </param>
        /// <param name="sourceDirectory">
        ///     Expected to be a fully qualified path to the directory when the file is stored.
        ///     If omitted, the directory path will be inferred from the caller's file path.
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
	        [CallerMemberName] string fileNameOrHint = UseNamingStrategy,
            [CallerFilePath] string sourceDirectory = UseCallersFilePath)
        {
	        var actualPath = _coordinator.GetFilePath(fileNameOrHint, sourceDirectory);

            ThrowIfFileMissing(actualPath);

            return actualPath;
        }

        /// <summary>
        ///     Retrieves the content of a text file.
        /// </summary>
        /// <param name="fileNameOrHint">
        ///     Explicitly specifies the file to open(filename + extension).
        ///     When omitted, the file name will be implied from the calling method's name.
        /// </param>
        /// <param name="sourceDirectory">
        ///     Expected to be a fully qualified path to the directory when the file is stored.
        ///     If omitted, the directory path will be inferred from the caller's file path.
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
	        [CallerMemberName] string fileNameOrHint = "",
            [CallerFilePath] string sourceDirectory = "")
        {
	        var actualPath = _coordinator.GetFilePath(fileNameOrHint, sourceDirectory);

            ThrowIfFileMissing(actualPath);

            return _osFile.ReadAllText(actualPath);
        }

        /// <summary>
        ///     Retrieves the content of a text file.
        /// </summary>
        /// <param name="fileNameOrHint">
        ///     Explicitly specifies the file to open(filename + extension).
        ///     When omitted, the file name will be implied from the calling method's name.
        /// </param>
        /// <param name="sourceDirectory">
        ///     Expected to be a fully qualified path to the directory when the file is stored.
        ///     When omitted, the directory path will be inferred from the caller's file path.
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
	        [CallerMemberName] string fileNameOrHint = "",
            [CallerFilePath] string sourceDirectory = "")
        {
	        var actualPath = _coordinator.GetFilePath(fileNameOrHint, sourceDirectory);

			ThrowIfFileMissing(actualPath);

            return _osFile.OpenRead(actualPath);
        }

        /// <summary>
        ///     Retrieves the content of a text file.
        /// </summary>
        /// <param name="fileNameOrHint">
        ///     Explicitly specifies the file to open(filename + extension).
        ///     When omitted, the file name will be implied from the calling method's name.
        /// </param>
        /// <param name="sourceDirectory">
        ///     Expected to be a fully qualified path to the directory when the file is stored.
        ///     When omitted, the directory path will be inferred from the caller's file path.
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
	        [CallerMemberName] string fileNameOrHint = "",
            [CallerFilePath] string sourceDirectory = "")
        {
	        var actualPath = _coordinator.GetFilePath(fileNameOrHint, sourceDirectory);

			ThrowIfFileMissing(actualPath);

            return new System.IO.StreamReader(_osFile.OpenRead(actualPath));
        }
    }
}
