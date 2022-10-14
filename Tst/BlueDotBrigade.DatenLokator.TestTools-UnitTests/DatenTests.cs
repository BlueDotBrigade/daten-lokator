namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System;
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
		private const string DefaultFileName = "DefaultInput.txt";
		private Coordinator _coordinator;

		[TestInitialize]
		public void TestInitialize()
		{
			var osDirectory = new OsDirectory();
			var osFile = new OsFile();

			_coordinator = new Coordinator(
				osFile,
				new AssertActArrange(),
				new SubFolderThenGlobal(osDirectory, osFile),
				new Dictionary<string, object>(),
				DefaultFileName);

			_coordinator.Setup();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			// nothing to do
		}

		[TestMethod]
		public void AsFilePath_FileByName_ReturnsPath()
		{
			var path = new Daten(_coordinator).AsFilePath("FileByName.txt");

			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsFilePath_FileByConvention_ReturnsPath()
		{
			var path = new Daten(_coordinator).AsFilePath();

			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsFilePath_FileByDefault_ReturnsPath()
		{
			var path = new Daten(_coordinator).AsFilePath(From.GlobalDefault);

			Assert.IsTrue(File.Exists(path));
			Assert.IsTrue(path.EndsWith(DefaultFileName));
			Assert.IsTrue(path.IndexOf("~Global", StringComparison.Ordinal) > 0);
		}

		[TestMethod]
		public void AsString_FileByDefaultName_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				"Default source file shared by multiple tests.",
				new Daten(_coordinator).AsString(From.GlobalDefault));
		}

		[TestMethod]
		public void AsString_FileByName_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				"Asked for file by name.", 
				new Daten(_coordinator).AsString("FileByName.txt"));
		}

		[TestMethod]
		public void AsString_FileByConvention_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				new Daten(_coordinator).AsString(),
				"Guessed file using test method's name.");
		}

		[TestMethod]
		public void AsString_MissingFileExtensionByConvention_ReturnsCorrectContent()
		{
			Assert.AreEqual("Missing file extension still works.", new Daten(_coordinator).AsString());
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_FileDoesNotExist_ThrowsFileNotFound()
		{
			new Daten(_coordinator).AsString("ThisFileCannotBeFound.txt");
		}
	}
}