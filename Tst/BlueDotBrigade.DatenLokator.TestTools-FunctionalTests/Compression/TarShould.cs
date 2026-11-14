namespace BlueDotBrigade.DatenLokator.TestTools.Compression
{
	[TestClass]
	public class TarShould
	{
		[TestMethod]
		public void AsString_CompressedFileIsLocal_ReturnsFileContent()
		{
			var fileContent = new Daten().AsString();

			Assert.AreEqual(
				"File content stored in local *.tar file.",
				fileContent);
		}

		[TestMethod]
		public void AsString_CompressedFileIsGlobal_ReturnsFileContent()
		{
			var fileContent = new Daten().AsString("TarIsGlobal.txt");

			Assert.AreEqual(
				"File content stored in global cache *.tar file.",
				fileContent);
		}
	}
}
