namespace BlueDotBrigade.DatenLokator.TestsTools.Strategies
{
	public interface ITestNamingStrategy
	{
		bool TryGetFileName(string hint, out string fileName);
	}
}