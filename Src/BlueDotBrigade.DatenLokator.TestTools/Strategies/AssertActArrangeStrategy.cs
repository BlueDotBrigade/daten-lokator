namespace BlueDotBrigade.DatenLokator.TestsTools.Strategies
{
	internal class AssertActArrangeStrategy : ITestNamingStrategy
	{
		public bool TryGetFileName(string hint, out string fileName)
		{
			var canParse = false;

			fileName = string.Empty;

			var testName = hint.Split('_');
			if (testName.Length == 3)
			{
				fileName = $"{testName[(int)AssertActArrangeParts.Scenario]}";
				canParse = true;
			}

			return canParse;
		}
	}
}
