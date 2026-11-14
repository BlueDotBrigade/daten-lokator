namespace BlueDotBrigade.DatenLokator.TestTools.Compression
{
	[TestClass]
	public class SevenZipShould
	{
		[TestMethod]
		public void AsString_CompressedFileIsLocal_ReturnsFileContent()
		{
			var fileContent = new Daten().AsString();

			Assert.AreEqual(
				"File content stored in local *.7z file.",
				fileContent);
		}

		[TestMethod]
		public void AsString_CompressedFileIsGlobal_ReturnsFileContent()
		{
			var fileContent = new Daten().AsString("SevenZipIsGlobal.txt");

			Assert.AreEqual(
				"File content stored in global cache *.7z file.",
				fileContent);
		}
	}
}
