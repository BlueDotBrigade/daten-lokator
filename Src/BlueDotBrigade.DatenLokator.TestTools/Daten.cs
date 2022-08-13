namespace BlueDotBrigade.DatenLokator.TestsTools
{
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;

	public class Daten
	{
		private readonly FileManager _fileManager;

		public Daten()
		{
			_fileManager = Lokator.Settings.FileManager;
		}

		/// <summary>
		/// Initializes an instance of the <see cref="Daten"/> class, to be used by automated testing.
		/// </summary>
		internal Daten(FileManager fileManager)
		{
			_fileManager = fileManager;
		}

		private static void ThrowIfFileMissing(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                var sourceFile = System.IO.Path.GetFileName(path);
                var directoryPath = System.IO.Path.GetDirectoryName(path) + @"\";
                throw new System.IO.FileNotFoundException(
                    $@"Unable to find the requested input file. Directory=`{directoryPath}`, File=`{sourceFile}`",
                    path);
            }

            System.Console.WriteLine($"Input data source has been selected. Name=`{System.IO.Path.GetFileName(path)}`");
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
	        [CallerMemberName] string fileNameOrHint = "",
            [CallerFilePath] string sourceDirectory = "")
        {
	        var actualPath = _fileManager.GetFilePath(fileNameOrHint, sourceDirectory);

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
	        var actualPath = _fileManager.GetFilePath(fileNameOrHint, sourceDirectory);

            ThrowIfFileMissing(actualPath);

            return System.IO.File.ReadAllText(actualPath);
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
	        var actualPath = _fileManager.GetFilePath(fileNameOrHint, sourceDirectory);

			ThrowIfFileMissing(actualPath);

            return System.IO.File.OpenRead(actualPath);
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
	        var actualPath = _fileManager.GetFilePath(fileNameOrHint, sourceDirectory);

			ThrowIfFileMissing(actualPath);

            return new System.IO.StreamReader(System.IO.File.OpenRead(actualPath));
        }
    }
}
