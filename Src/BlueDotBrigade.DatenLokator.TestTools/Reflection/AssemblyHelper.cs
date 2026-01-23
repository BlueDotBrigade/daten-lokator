namespace BlueDotBrigade.DatenLokator.TestTools.Reflection
{
    using System;
	using BlueDotBrigade.DatenLokator.TestTools.Configuration;
	using System.IO;

    internal static class AssemblyHelper
    {
		/// <summary>
		///     Returns a fully qualified path for the executing assembly.
		/// </summary>
		/// <remarks>
		///     While path does include the parent directory name, it does not include the name of the assembly.
		/// </remarks>
		internal static string ExecutingDirectory
        {
            get
            {
                var codeBase = AppDomain.CurrentDomain.BaseDirectory;
                var uriBuilder = new UriBuilder(new Uri(codeBase));
                var path = Uri.UnescapeDataString(uriBuilder.Path);

                // Normalize path separators to match the platform (fixes Windows vs Linux differences)
                path = path
					.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
					.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);

                return path;
            }
        }

		internal static string ProjectDirectoryPath
		{
			get
			{
				return GetProjectDirectoryPath(ExecutingDirectory);
			}
		}

		internal static string GetProjectDirectoryPath(string executingDirectory)
		{
			var normalizedDirectory = executingDirectory;

			normalizedDirectory = normalizedDirectory
				.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
				.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar)
				.TrimEnd(Path.DirectorySeparatorChar);

			var binToken = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";
			var binIndex = normalizedDirectory.LastIndexOf(
				binToken,
				StringComparison.InvariantCultureIgnoreCase);

			if (binIndex < 0)
			{
				var binSuffix = $"{Path.DirectorySeparatorChar}bin";
				if (normalizedDirectory.EndsWith(binSuffix, StringComparison.InvariantCultureIgnoreCase))
				{
					binIndex = normalizedDirectory.Length - binSuffix.Length;
				}
			}

			if (binIndex < 0)
			{
				return normalizedDirectory;
			}

			return normalizedDirectory.Substring(0, binIndex);
		}

		internal static string DefaultDatenDirectoryPath
		{
			get
			{
				return Path.Combine(
					ProjectDirectoryPath, 
					LokatorConfiguration.RootDirectoryNameDefault);
			}
		}
    }
}
