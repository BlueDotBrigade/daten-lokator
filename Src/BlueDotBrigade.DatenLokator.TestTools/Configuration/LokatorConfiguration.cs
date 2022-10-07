namespace BlueDotBrigade.DatenLokator.TestsTools.Configuration
{
	using System;
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	internal class LokatorConfiguration
    {
		/// <summary>
		/// Represents the name of the root directory that all input data is stored within.
		/// </summary>
		/// <remarks>
		/// By default, this directory is assumed to be in the same folder as `.csproj`.
		/// </remarks>
		public const string RootDirectoryName = ".Daten";

		private static readonly IDictionary<string, object> NoProperties = new Dictionary<string, object>();

		private IDictionary<string, object> _testEnvironmentProperties;
	    private string _defaultFilePath;

	    private ITestNamingStrategy _testNamingStrategy;
		private IFileManagementStrategy _fileManagementStrategy;

		public LokatorConfiguration(ITestNamingStrategy testNamingStrategy, IFileManagementStrategy fileManagementStrategy)
	    {
		    _testEnvironmentProperties = NoProperties;
			_defaultFilePath = string.Empty;

			_testNamingStrategy = testNamingStrategy;
			_fileManagementStrategy = fileManagementStrategy;
	    }

		/// <summary>
		/// Represents the key that is used to find the root directory path in the <see cref="TestContext"/> dictionary.
		/// </summary>
		internal const string RootDirectoryKey = "DatenLokatorRootPath";

		internal IDictionary<string, object> TestEnvironmentProperties
		{
			get
			{
				return _testEnvironmentProperties;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(this.TestEnvironmentProperties));
				}
				else
				{
					_testEnvironmentProperties = value;
				}
			}
		}

		internal string DefaultFilePath
	    {
		    get
		    {
			    return _defaultFilePath;
		    }
		    set
		    {
			    if (value == null)
			    {
				    throw new ArgumentNullException(nameof(this.DefaultFilePath));
			    }
			    else
			    {
				    _defaultFilePath = value;
			    }
		    }
	    }

		internal ITestNamingStrategy TestNamingStrategy
		{
			get
			{
				return _testNamingStrategy;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(this.TestNamingStrategy));
				}
				else
				{
					_testNamingStrategy = value;
				}
			}
		}

		internal IFileManagementStrategy FileManagementStrategy 
		{
			get
			{
				return _fileManagementStrategy;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(this.FileManagementStrategy));
				}
				else
				{
					_fileManagementStrategy = value;
				}
			}
		}
    }
}
