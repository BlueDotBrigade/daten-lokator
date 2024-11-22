namespace BlueDotBrigade.DatenLokator.TestTools.Configuration
{
	using System;
	using System.Collections.Generic;
	using BlueDotBrigade.DatenLokator.TestTools.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	internal class LokatorConfiguration
    {
		/// <summary>
		/// Represents the name of the root directory where all of the input data is stored.
		/// </summary>
		/// <remarks>
		/// By default, this folder is assumed to be within the .NET project (`.csproj`) directory.
		/// </remarks>
		public const string RootDirectoryNameDefault = ".Daten";

		public const string DefaultFileNameDefault = "Default.txt";

		private static readonly IDictionary<string, object> NoProperties = new Dictionary<string, object>();

		private IDictionary<string, object> _testEnvironmentProperties;
	    private string _defaultFileName;

	    private ITestNamingStrategy _testNamingStrategy;
		private IFileManagementStrategy _fileManagementStrategy;

		public LokatorConfiguration(ITestNamingStrategy testNamingStrategy, IFileManagementStrategy fileManagementStrategy)
	    {
		    _testEnvironmentProperties = NoProperties;
			_defaultFileName = DefaultFileNameDefault;

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

		internal string DefaultFileName
	    {
		    get
		    {
			    return _defaultFileName;
		    }
		    set
		    {
			    if (value == null)
			    {
				    throw new ArgumentNullException(nameof(this.DefaultFileName));
			    }
			    else
			    {
				    _defaultFileName = value;
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
