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

                return path;
            }
        }

		internal static string ProjectDirectoryPath
		{
			get
			{
				var binIndex =  ExecutingDirectory.LastIndexOf(
					$"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", 
					StringComparison.InvariantCultureIgnoreCase);

				var projectPath = ExecutingDirectory.Substring(
					0,
					binIndex);

				return projectPath;
			}
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