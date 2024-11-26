namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	public class AsIsNamingStrategy : ITestNamingStrategy
	{
		public bool TryGetFileName(string methodOrFileName, out string fileName)
		{
			fileName = methodOrFileName;
			return true;
		}
	}
}