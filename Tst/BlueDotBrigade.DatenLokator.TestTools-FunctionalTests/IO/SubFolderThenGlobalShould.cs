namespace BlueDotBrigade.DatenLokator.TestTools.IO
{
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
	using Moq;

	[TestClass]
	public class SubFolderThenGlobalShould
	{

		[TestMethod]
		public void RetrieveFileFromDefaultGlobalDirectory()
		{
			//var binDirectoryPath = new ProcessHelper().GetTestEngineDirectory();
			
			//var fileName = "Fake.log";
			//var filePath = $@"{directoryPath}\Fake.log";

			//var namingStrategy = new Mock<ITestNamingStrategy>();
			//namingStrategy.Setup(x => x.TryGetFileName(It.IsAny<string>(), out fileName))
			//	.Returns(true);

			//try
			//{
			//	Directory.CreateDirectory(directoryPath);
			//	File.AppendAllText(filePath, "This is a sample log file.");

			//	ProcessHelper

			//	var fileManager = new SubFolderThenGlobal();
			//	fileManager.Setup(
			//		new OsDirectory(),
			//		new OsFile(),
			//		directoryPath);

			//	// 1. if global path set, then return file in global directory
			//	// 2. if global path NOT set, then default to the executable directory

			//	fileManager.GetFilePath(namingStrategy.Object, "filenameOrHint", "sourceDirectory");

			//	//Assert.AreEqual(fileManager.GlobalDirectoryPath);
			//}
			//finally
			//{
			//	if (File.Exists(filePath))
			//	{
			//		File.Delete(filePath);
			//	}

			//	if (Directory.Exists(directoryPath))
			//	{
			//		Directory.Delete(directoryPath);
			//	}
			//}
		}

		[TestMethod]
		public void RetrieveFileFromCustomGlobalDirectory()
		{
			//var directoryPath = Path.Combine(Path.GetTempPath(), @"DatenLokatorGlobalFiles");

			//var fileName = "Fake.log";
			//var filePath = $@"{directoryPath}\Fake.log";

			//var namingStrategy = new Mock<ITestNamingStrategy>();
			//namingStrategy.Setup(x => x.TryGetFileName(It.IsAny<string>(), out fileName))
			//	.Returns(true);

			//try
			//{
			//	Directory.CreateDirectory(directoryPath);
			//	File.AppendAllText(filePath, "This is a sample log file.");

			//	var fileManager = new SubFolderThenGlobal();
			//	fileManager.Setup(
			//		new OsDirectory(),
			//		new OsFile(),
			//		directoryPath);

			//	// 1. if global path set, then return file in global directory
			//	// 2. if global path NOT set, then default to the executable directory

			//	fileManager.GetFilePath(namingStrategy.Object, "filenameOrHint", "sourceDirectory");

			//	Assert.AreEqual(fileManager.GlobalDirectoryPath);
			//}
			//finally
			//{
			//	if (File.Exists(filePath))
			//	{
			//		File.Delete(filePath);
			//	}

			//	if (Directory.Exists(directoryPath))
			//	{
			//		Directory.Delete(directoryPath);
			//	}
			//}
		}
	}
}
