namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System.Collections.Generic;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
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
				new AssertActArrange(),
				new SubFolderThenGlobal(osDirectory, osFile),
				new Dictionary<string, object>(),
				"DefaultInput.txt"); 

			_lokator = new Lokator(osDirectory, osFile);
			_lokator.UsingDefaultFileName("Default.txt");// duplicate
			_lokator.Setup();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			// nothing to do
		}

		[TestMethod]
		public void AsFilePath_FileByName_ReturnsPath()
		{
			var path = new Daten(_lokator).AsFilePath("FileByName.txt");

			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsFilePath_FileByConvention_ReturnsPath()
		{
			var path = new Daten(_lokator).AsFilePath();

			Assert.IsTrue(File.Exists(path));
		}


		[TestMethod]
		public void AsString_FileByName_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				"Asked for file by name.", 
				new Daten(_lokator).AsString("FileByName.txt"));
		}

		[TestMethod]
		public void AsString_UsingDefaultFile_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				"Default source file shared by multiple tests.",
				new Daten(_lokator).AsString(Using.DefaultFileName));
		}

		[TestMethod]
		public void AsString_FileByConvention_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				new Daten(_lokator).AsString(),
				"Guessed file using test method's name.");
		}

		[TestMethod]
		public void AsString_MissingFileExtensionByConvention_ReturnsCorrectContent()
		{
			Assert.AreEqual("Missing file extension still works.", new Daten(_lokator).AsString());
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_FileDoesNotExist_ThrowsFileNotFound()
		{
			new Daten(_lokator).AsString("ThisFileCannotBeFound.txt");
		}
	}
}