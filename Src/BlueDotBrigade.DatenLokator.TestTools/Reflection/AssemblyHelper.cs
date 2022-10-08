namespace BlueDotBrigade.DatenLokator.TestsTools.Reflection
{
    using System;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
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

                return path.Replace(@"/", @"\");
            }
        }

		internal static string ProjectDirectoryPath
		{
			get
			{
				var index = ExecutingDirectory.LastIndexOf(@"\bin\");
				var projectDirectoryPath = AssemblyHelper.ExecutingDirectory.Substring(0, index);

				return Path.Combine(projectDirectoryPath, LokatorConfiguration.RootDirectoryNameDefault);
			}
		}
    }
}