namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class DatenTests
	{
		private Lokator _lokator;

		[TestInitialize]
		public void TestInitialize()
		{
			var osDirectory = new OsDirectory();
			var osFile = new OsFile();

			var coordinator = new Coordinator(
				new SubFolderThenGlobal(osDirectory, osFile),
				new AssertActArrange(),
				"Default.txt");

			_lokator = new Lokator(osDirectory, osFile);
			_lokator.Setup();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			// nothing to do
		}

		private static string GetProjectDirectory()
		{
			var projectDirectory = Path.Combine(AssemblyHelper.ExecutingDirectory, @"..\..\..\");

			// Remove relative path references (i.e. return a real path)
			return Path.GetFullPath(projectDirectory);
		}

		//[TestMethod]
		//public void BasePath_AssumedLocalPath_ReturnsPathToProjectFolder()
		//{
		//    Assert.IsTrue(string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["InputDataPath"]),
		//        $"This test requires that you do NOT define a `{nameof(new Daten(_coordinator).BaseDirectoryPath)}` for the input data. BaseDirectoryPath=`{new Daten(_coordinator).BaseDirectoryPath}`");

		//    Debug.WriteLine($"Base path is assumed to be: {new Daten(_coordinator).BaseDirectoryPath}");

		//    var expectedInputDataPath = Path.Combine(GetProjectDirectory(), new Daten(_coordinator).BaseDirectoryName);

		//    Assert.AreEqual(expectedInputDataPath, new Daten(_coordinator).BaseDirectoryPath);
		//}

		//[TestMethod]
		//public void BasePath_ExplicitlyDefinedPath_ReturnsPathToProjectFolder()
		//{
		//    var newInputDataFolder = "InputAlternateLocation";
		//    var originalValue = ConfigurationManager.AppSettings[new Daten(_coordinator).BasePathKey];

		//    try
		//    {
		//        ConfigurationManager.AppSettings[new Daten(_coordinator).BasePathKey] =
		//            Path.Combine(GetProjectDirectory(), newInputDataFolder);
		//        Debug.WriteLine($"Base path was explicitly set as: {new Daten(_coordinator).BaseDirectoryPath}");

		//        var newBasePath = Path.Combine(GetProjectDirectory(), newInputDataFolder);

		//        Assert.IsTrue(!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[new Daten(_coordinator).BasePathKey]));
		//        Assert.AreEqual(newBasePath, new Daten(_coordinator).BaseDirectoryPath);
		//    }
		//    finally
		//    {
		//        ConfigurationManager.AppSettings[new Daten(_coordinator).BasePathKey] = originalValue;
		//    }
		//}

		[TestMethod]
		public void AsFilePath_FileRequestedExplicitly_ReturnsPath()
		{
			var path = new Daten(_lokator).AsFilePath("FileRequestedExplicitly.txt");

			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsString_FileRequestedExplicitly_ReturnsFileContent()
		{
			Assert.AreEqual("NothingWasImplied", new Daten(_lokator).AsString("FileRequestedExplicitly.txt"));
		}

		[TestMethod]
		public void AsString_FileRequestedImplicitly_ReturnsFileContent()
		{
			Assert.AreEqual("FileNameImpliedByUnitTestName", new Daten(_lokator).AsString());
		}

		[TestMethod]
		public void AsString_FileRequestedImplicitlyHasExtension_ReturnsFileContent()
		{
			Assert.AreEqual("Convention Over Configuration", new Daten(_lokator).AsString());
		}

		//[TestMethod]
		//[ExpectedException(typeof(FileNotFoundException))]
		//public void AsString_FileDoesNotExist_ThrowsFileNotFound()
		//{
		//	new Daten(_lokator).AsString("FileDoesNotExistOnDisk.txt", @"c:\Invalid\Path\Here");
		//}
	}
}