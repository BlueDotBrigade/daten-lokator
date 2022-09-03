namespace BlueDotBrigade.DatenLokator.TestTools
{
	using BlueDotBrigade.DatenLokator.TestsTools;

	[TestClass]
	public class DatenShould
	{
		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_ImplicitFileDoesNotExist_Throws()
		{
			var fileContent = new Daten().AsString();

			Assert.Fail("An exception should have been thrown.");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void AsString_ExplicitFileDoesNotExist_Throws()
		{
			var fileContent = new Daten().AsString("ThisFileDoesNotExist.txt");

			Assert.Fail("An exception should have been thrown.");
		}

		[TestMethod]
		public void AsString_ImplicitFileName_ReturnsLocalFileContent()
		{
			var fileContent = new Daten().AsString();

			Assert.AreEqual(
				@"Implicit filename used to retrieve local file content.",
				fileContent);
		}

		[TestMethod]
		public void AsString_ExplicitFileName_ReturnsLocalFileContent()
		{
			var fileContent = new Daten().AsString(@"ExplicitFileName.log");

			Assert.AreEqual(
				@"Explicit filename used to retrieve local file content.",
				fileContent);
		}

		[TestMethod]
		public void AsString_ImplicitFile_ReturnsGlobalFileContent()
		{
			var fileContent = new Daten().AsString();

			Assert.AreEqual(
				@"Implicit filename used to retrieve global file content.",
				fileContent);
		}

		[TestMethod]
		public void AsString_ExplicitFile_ReturnsGlobalFileContent()
		{
			var fileContent = new Daten().AsString(@"ExplicitFile.csv");

			Assert.AreEqual(
				@"Explicit filename used to retrieve global file content.",
				fileContent);
		}
	}
}
