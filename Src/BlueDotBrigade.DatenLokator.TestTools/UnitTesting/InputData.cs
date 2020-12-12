namespace BlueDotBrigade.DatenLokator.TestsTools.UnitTesting
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using BlueDotBrigade.DatenLokator.TestsTools.IO;
    using BlueDotBrigade.DatenLokator.TestsTools.Reflection;


    /// <summary>
    ///     Uses "Convention Over Configuration" to simplify the management of automated test data.
    /// </summary>
    public static class InputData
    {
        /// <summary>
        ///     Represents the application setting's key that can be used to explicitly set the <see cref="BaseDirectoryName" />
        ///     path.
        /// </summary>
        /// <seealso cref="ConfigurationManager.AppSettings" />
        internal const string BasePathKey = InputPathSelector.BasePathKey;

        /// <summary>
        ///     Represents the name of the sub-directory where all of the input data will be stored.
        /// </summary>
        public const string BaseDirectoryName = InputPathSelector.BaseDirectoryName;

        private static readonly InputPathSelector Selector = new InputPathSelector(
            new Directory(),
            new File(),
            ConfigurationManager.AppSettings,
            AssemblyHelper.ExecutingDirectory);

        public static string BaseDirectoryPath => Selector.BaseDirectoryPath;

        public static void Setup()
        {
            Selector.Setup();
        }

        public static void Decompress(string directoryPath)
        {
            Selector.Decompress(directoryPath);
        }

        /// <seealso cref="Setup" />
        public static void Teardown()
        {
            Selector.Teardown();
        }

        private static void VerifyFileExistence(string path)
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
        /// <param name="sourceFile">
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
        public static string GetFilePath([CallerMemberName] string sourceFile = "",
	        [CallerFilePath] string sourceDirectory = "")
        {
	        var actualFilePath = Selector.GetFilePathOrInfer(sourceFile, sourceDirectory);

	        VerifyFileExistence(actualFilePath);

	        return actualFilePath;
        }

        /// <summary>
        ///     Retrieves the content of a text file.
        /// </summary>
        /// <param name="sourceFile">
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
        public static string GetAsString([CallerMemberName] string sourceFile = "",
            [CallerFilePath] string sourceDirectory = "")
        {
            var actualFilePath = Selector.GetFilePathOrInfer(sourceFile, sourceDirectory);

            VerifyFileExistence(actualFilePath);

            return System.IO.File.ReadAllText(actualFilePath);
        }

		/// <summary>
		///     Retrieves the content of a text file.
		/// </summary>
		/// <param name="sourceFile">
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
        public static System.IO.Stream GetAsStream([CallerMemberName] string sourceFile = "",
            [CallerFilePath] string sourceDirectory = "")
        {
            var actualFilePath = Selector.GetFilePathOrInfer(sourceFile, sourceDirectory);

            VerifyFileExistence(actualFilePath);

            return System.IO.File.OpenRead(actualFilePath);
        }

		/// <summary>
		///     Retrieves the content of a text file.
		/// </summary>
		/// <param name="sourceFile">
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
        public static System.IO.StreamReader GetAsStreamReader([CallerMemberName] string sourceFile = "",
            [CallerFilePath] string sourceDirectory = "")
        {
            var actualFilePath = Selector.GetFilePathOrInfer(sourceFile, sourceDirectory);

            VerifyFileExistence(actualFilePath);

            return new System.IO.StreamReader(System.IO.File.OpenRead(actualFilePath));
        }
    }
}