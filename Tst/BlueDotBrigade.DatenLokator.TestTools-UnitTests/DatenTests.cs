namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System.Collections.Generic;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class DatenTests
	{
		private Coordinator _fileManager;

		[TestInitialize]
		public void TestInitialize()
		{
			var noSettings = new Dictionary<string, object>();

			_fileManager = new Coordinator(new SubFolderThenGlobal(), new AssertActArrange());
			_fileManager.Setup(
				new OsDirectory(),
				new OsFile(),
				AssemblyHelper.ExecutingDirectory,
				noSettings);
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
		//        $"This test requires that you do NOT define a `{nameof(new Daten(_fileManager).BaseDirectoryPath)}` for the input data. BaseDirectoryPath=`{new Daten(_fileManager).BaseDirectoryPath}`");

		//    Debug.WriteLine($"Base path is assumed to be: {new Daten(_fileManager).BaseDirectoryPath}");

		//    var expectedInputDataPath = Path.Combine(GetProjectDirectory(), new Daten(_fileManager).BaseDirectoryName);

		//    Assert.AreEqual(expectedInputDataPath, new Daten(_fileManager).BaseDirectoryPath);
		//}

		//[TestMethod]
		//public void BasePath_ExplicitlyDefinedPath_ReturnsPathToProjectFolder()
		//{
		//    var newInputDataFolder = "InputAlternateLocation";
		//    var originalValue = ConfigurationManager.AppSettings[new Daten(_fileManager).BasePathKey];

		//    try
		//    {
		//        ConfigurationManager.AppSettings[new Daten(_fileManager).BasePathKey] =
		//            Path.Combine(GetProjectDirectory(), newInputDataFolder);
		//        Debug.WriteLine($"Base path was explicitly set as: {new Daten(_fileManager).BaseDirectoryPath}");

		//        var newBasePath = Path.Combine(GetProjectDirectory(), newInputDataFolder);

		//        Assert.IsTrue(!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[new Daten(_fileManager).BasePathKey]));
		//        Assert.AreEqual(newBasePath, new Daten(_fileManager).BaseDirectoryPath);
		//    }
		//    finally
		//    {
		//        ConfigurationManager.AppSettings[new Daten(_fileManager).BasePathKey] = originalValue;
		//    }
		//}

		[TestMethod]
		public void AsFilePath_FileRequestedExplicitly_ReturnsPath()
		{
			var path = new Daten(_fileManager).AsFilePath("FileRequestedExplicitly.txt");
			
			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsString_FileRequestedExplicitly_ReturnsFileContent()
		{
			Assert.AreEqual("NothingWasImplied", new Daten(_fileManager).AsString("FileRequestedExplicitly.txt"));
		}

		[TestMethod]
		public void AsString_FileRequestedImplicitly_ReturnsFileContent()
		{
			Assert.AreEqual("FileNameImpliedByUnitTestName", new Daten(_fileManager).AsString());
		}

		[TestMethod]
		public void AsString_FileRequestedImplicitlyHasExtension_ReturnsFileContent()
		{
			Assert.AreEqual("Convention Over Configuration", new Daten(_fileManager).AsString());
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_FileDoesNotExist_ThrowsFileNotFound()
		{
			new Daten(_fileManager).AsString("FileDoesNotExistOnDisk.txt", @"c:\Invalid\Path\Here");
		}
	}
}