namespace BlueDotBrigade.DatenLokator.TestTools.IO
{
	using System;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;

	[TestClass]
	public class SubFolderThenGlobalTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Setup_InvalidPath_Throws()
		{
			var osFile = new Mock<IOsFile>();
			var osDirectory = new Mock<IOsDirectory>();

			osDirectory
				.Setup(d => d.Exists(It.IsAny<string>()))
				.Returns(false);

			var fileManager = new SubFolderThenGlobal(osDirectory.Object, osFile.Object);
			fileManager.Setup(@"C:\This\Path\Does\Not\Exist");

			Assert.Fail("Setup should have thrown an exception.");
		}

		[TestMethod]
		public void Setup_ValidPath_SetsRootDirectoryPath()
		{
			var osFile = new Mock<IOsFile>();
			var osDirectory = new Mock<IOsDirectory>();

			osDirectory
				.Setup(d => d.Exists(It.IsAny<string>()))
				.Returns(true);

			var fileManager = new SubFolderThenGlobal(osDirectory.Object, osFile.Object);
			fileManager.Setup(@"C:\New\Root\Directory\Path");

			Assert.AreEqual(
				@"C:\New\Root\Directory\Path",
				fileManager.RootDirectoryPath);
		}

		[TestMethod]
		public void Setup_ValidPath_SetsGlobalDirectoryPath()
		{
			var osFile = new Mock<IOsFile>();
			var osDirectory = new Mock<IOsDirectory>();

			osDirectory
				.Setup(d => d.Exists(It.IsAny<string>()))
				.Returns(true);

			var fileManager = new SubFolderThenGlobal(osDirectory.Object, osFile.Object);
			fileManager.Setup(@"C:\New\Root\Directory\Path");
			
			Assert.AreEqual(
				@"C:\New\Root\Directory\Path\~Global",
				fileManager.GlobalDirectoryPath);
		}

		[TestMethod]
		public void GetFilePath_NewRootDirectory_ReturnsCorrectPath()
		{
			var thisClassPath = Path.Combine(AssemblyHelper.ProjectDirectoryPath, @"IO\SubFolderThenGlobalTests.cs");

			var osFile = new Mock<IOsFile>();
			osFile
				.Setup(d => d.Exists(It.IsAny<string>()))
				.Returns(true);

			var osDirectory = new Mock<IOsDirectory>();
			osDirectory
				.Setup(d => d.Exists(It.IsAny<string>()))
				.Returns(true);

			var namingStrategy = new Mock<ITestNamingStrategy>();

			var fileManager = new SubFolderThenGlobal(osDirectory.Object, osFile.Object);
			fileManager.Setup(@"C:\SampleData\");

			Assert.AreEqual(
				@"C:\SampleData\IO\SubFolderThenGlobalTests\FooBar.txt",
				fileManager.GetFilePath(
					namingStrategy.Object,
					"FooBar.txt",
					thisClassPath));
		}
	}
}