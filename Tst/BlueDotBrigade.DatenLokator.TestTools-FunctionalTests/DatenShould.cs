namespace BlueDotBrigade.DatenLokator.TestTools
{
	using BlueDotBrigade.DatenLokator.TestsTools;

	[TestClass]
	public class DatenShould
	{
		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_ByConventionFileDoesNotExist_Throws()
		{
			var fileContent = new Daten().AsString();

			Assert.Fail("An exception should have been thrown.");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_ByFileNameFileDoesNotExist_Throws()
		{
			var fileContent = new Daten().AsString("ThisFileDoesNotExist.txt");

			Assert.Fail("An exception should have been thrown.");
		}

		[TestMethod]
		public void AsString_ByDefault_ReturnsDefaultFileContent()
		{
			var fileContent = new Daten().AsString(From.GlobalDefault);

			Assert.AreEqual(
				@"DatenLokator will default to this filename.",
				fileContent);
		}

		[TestMethod]
		public void AsString_ByConvention_ReturnsLocalFileContent()
		{
			var fileContent = new Daten().AsString();

			Assert.AreEqual(
				@"The name of the test is used to determine the source file name.",
				fileContent);
		}

		[TestMethod]
		public void AsString_ByFileName_ReturnsLocalFileContent()
		{
			var fileContent = new Daten().AsString(@"ByFileName.log");

			Assert.AreEqual(
				@"DatenLokator is explicitly told the name of the source file.",
				fileContent);
		}

		[TestMethod]
		public void AsString_ByConventionGlobalData_ReturnsGlobalFileContent()
		{
			var fileContent = new Daten().AsString();

			var expected =
				@"The name of the test is used to determine the source file name. " +
				@"Since file does not exist locally, DatenLokator will check the global cache.";

			Assert.AreEqual(
				expected,
				fileContent);
		}

		[TestMethod]
		public void AsString_ByFileNameGlobalData_ReturnsGlobalFileContent()
		{
			var fileContent = new Daten().AsString(@"ByFileNameGlobalData.md");

			Assert.AreEqual(
				@"Provided file name could not be found locally, so DatenLokator will check the global cache.",
				fileContent);
		}
	}
}
