namespace BlueDotBrigade.DatenLokator.TestTools.IO
{
	using System;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using BlueDotBrigade.DatenLokator.TestTools.Reflection;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using NSubstitute;

	[TestClass]
	public class SubFolderThenGlobalTests
	{
		private static string NormalizePath(string path)
		{
			if (Path.DirectorySeparatorChar == '/')
			{
				return path.Replace('\\', '/');
			}

			return path.Replace('/', '\\');
		}

		[TestMethod]
		public void Setup_InvalidPath_Throws()
		{
			var osFile = Substitute.For<IOsFile>();
			var osDirectory = Substitute.For<IOsDirectory>();

			osDirectory
				.Exists(Arg.Any<string>())
				.Returns(false);

			var fileManager = new SubFolderThenGlobal(osDirectory, osFile);

			Assert.Throws<ArgumentException>(() =>
			{
				fileManager.Setup(@"C:\This\Path\Does\Not\Exist");
			});
		}

		[TestMethod]
		public void Setup_ValidPath_SetsRootDirectoryPath()
		{
			var osFile = Substitute.For<IOsFile>();
			var osDirectory = Substitute.For<IOsDirectory>();

			osDirectory
				.Exists(Arg.Any<string>())
				.Returns(true);

			var fileManager = new SubFolderThenGlobal(osDirectory, osFile);
			fileManager.Setup(@"C:\New\Root\Directory\Path");

			Assert.AreEqual(
				NormalizePath(@"C:\New\Root\Directory\Path"),
				fileManager.RootDirectoryPath);
		}

		[TestMethod]
		public void Setup_ValidPath_SetsGlobalDirectoryPath()
		{
			var osFile = Substitute.For<IOsFile>();
			var osDirectory = Substitute.For<IOsDirectory>();

			osDirectory
				.Exists(Arg.Any<string>())
				.Returns(true);

			var fileManager = new SubFolderThenGlobal(osDirectory, osFile);
			fileManager.Setup(@"C:\New\Root\Directory\Path");

			Assert.AreEqual(
				Path.Combine(NormalizePath(@"C:\New\Root\Directory\Path"), ".Global"),
				fileManager.GlobalDirectoryPath);
		}

		[TestMethod]
		public void Setup_WindowsPathOnUnix_NormalizesSeparators()
		{
			if (Path.DirectorySeparatorChar != '/')
			{
				Assert.Inconclusive("Test only applies on Unix-like systems.");
			}

			var osFile = Substitute.For<IOsFile>();
			var osDirectory = Substitute.For<IOsDirectory>();

			osDirectory
				.Exists(Arg.Any<string>())
				.Returns(true);

			var fileManager = new SubFolderThenGlobal(osDirectory, osFile);
			fileManager.Setup(@"/home/runner/work/demo/.Daten");

			Assert.AreEqual(
				"/home/runner/work/demo/.Daten",
				fileManager.RootDirectoryPath);
		}

		[TestMethod]
		public void GetFilePath_NewRootDirectory_ReturnsCorrectPath()
		{
			var thisClassPath = Path.Combine(AssemblyHelper.ProjectDirectoryPath, "IO", "SubFolderThenGlobalTests.cs");

			var osFile = Substitute.For<IOsFile>();
			osFile
				.Exists(Arg.Any<string>())
				.Returns(true);

			var osDirectory = Substitute.For<IOsDirectory>();
			osDirectory
				.Exists(Arg.Any<string>())
				.Returns(true);
			var namingStrategy = Substitute.For<ITestNamingStrategy>();

			var fileManager = new SubFolderThenGlobal(osDirectory, osFile);
			fileManager.Setup(@"C:\SampleData\");

			var normalizedRoot = NormalizePath(@"C:\SampleData\").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			Assert.AreEqual(
				Path.Combine(normalizedRoot, "IO", "SubFolderThenGlobalTests", "FooBar.txt"),
				fileManager.GetFilePath(
					namingStrategy,
					"FooBar.txt",
					thisClassPath));
		}
	}
}
