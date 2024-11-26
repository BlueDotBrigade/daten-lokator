namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	public class GenericNamingStrategy : ITestNamingStrategy
	{
		private readonly TryParse _tryParse;

		public delegate bool TryParse(string methodName, out string fileName);

		public GenericNamingStrategy(TryParse tryParse)
		{
			_tryParse = tryParse;
		}

		public bool TryGetFileName(string methodName, out string fileName)
		{
			fileName = string.Empty;
			return _tryParse(methodName, out fileName);
		}
	}
}
