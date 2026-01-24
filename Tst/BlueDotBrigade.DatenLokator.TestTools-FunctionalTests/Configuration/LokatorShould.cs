namespace BlueDotBrigade.DatenLokator.TestTools.Configuration
{
	using BlueDotBrigade.DatenLokator.TestTools.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using NSubstitute;
	using System.Collections.Generic;

	/// <summary>
	/// These test methods are more about exploring the <see cref="Lokator"/> API, to ensure that the library can be extended.
	/// </summary>
	[TestClass]
	public class LokatorShould
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
		public void UseDefaultRootDirectory()
		{
			var path = new Daten().AsFilePath("FooBar.txt");

			Assert.IsTrue(path.EndsWith(@"/.Daten/Configuration/LokatorShould/FooBar.txt") ||
				path.EndsWith(@"\.Daten\Configuration\LokatorShould\FooBar.txt"));
		}

		// Include tests that verify the search path

		[TestMethod]
		public void UseCustomRootDirectory()
		{
			const string CustomRootDirectory = @"Z:\InputData";

			// Emulating MS Test's TestContext which includes a `Properties` collection
			var properties = new Dictionary<string, object>
			{
				{ LokatorConfiguration.RootDirectoryKey, CustomRootDirectory }
			};

			var directory = Substitute.For<IOsDirectory>();
			directory.Exists(Arg.Any<string>()).Returns(true);

			var file = Substitute.For<IOsFile>();
			file.Exists(Arg.Any<string>()).Returns(callInfo =>
			{
				var path = callInfo.Arg<string>();
				var normalizedRoot = NormalizePath(CustomRootDirectory);
				return path.StartsWith(CustomRootDirectory) || path.StartsWith(normalizedRoot);
			});

			var coordinator = new Coordinator(
				file,
				new MemberCaseResultNamingStrategy(),
				new SubFolderThenGlobal(directory, file),
				new Dictionary<string, object>(),
				"Default.txt",
				CustomRootDirectory);

			coordinator.Setup();

			var sourceFilePath = new Daten(coordinator).AsFilePath("MyFile.log");

			var normalizedRootPath = NormalizePath(CustomRootDirectory);
			Assert.IsTrue(sourceFilePath.StartsWith(normalizedRootPath) ||
				sourceFilePath.StartsWith(CustomRootDirectory));
		}
	}
}
