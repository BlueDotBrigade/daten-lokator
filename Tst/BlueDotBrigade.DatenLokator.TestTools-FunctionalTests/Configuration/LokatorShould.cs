namespace BlueDotBrigade.DatenLokator.TestTools.Configuration
{
	using BlueDotBrigade.DatenLokator.TestsTools;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;

	/// <summary>
	/// These test methods are more about exploring the <see cref="Lokator"/> API, to ensure that the library can be extended.
	/// </summary>
	[TestClass]
	public class LokatorShould
	{
		[TestMethod]
		public void UseDefaultInputDirectory()
		{
			
		}

		// Include tests that verify the search path

		[TestMethod]
		public void UseSpecificRootDirectory()
		{
			const string CustomRootDirectory = @"Z:\InputData";

			// Emulating MS Test's TestContext which includes a `Properties` collection
			var properties = new Dictionary<string, object>
			{
				{ LokatorConfiguration.RootDirectoryKey, CustomRootDirectory }
			};

			var directory = new Mock<IOsDirectory>();
			directory.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

			var file = new Mock<IOsFile>();
			file.Setup(x => x.Exists(It.IsAny<string>())).Returns((string path) =>
			{
				return path.StartsWith(CustomRootDirectory);
			});

			Lokator lokator = new Lokator(directory.Object, file.Object)
				.UsingTestContext(properties)
				.Setup();

			var sourceFilePath = new Daten(lokator).AsFilePath("MyFile.log");

			Assert.IsTrue(sourceFilePath.StartsWith(CustomRootDirectory));
		}
	}
}
