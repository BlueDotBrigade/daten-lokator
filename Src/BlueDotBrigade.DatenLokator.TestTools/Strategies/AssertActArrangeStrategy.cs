﻿namespace BlueDotBrigade.DatenLokator.TestsTools.Strategies
{
	internal class AssertActArrangeStrategy : ITestNamingStrategy
	{
		public bool TryGetFileName(string methodOrFileName, out string fileName)
		{
			var canParse = false;

			fileName = string.Empty;

			var testName = methodOrFileName.Split('_');
			if (testName.Length == 3)
			{
				fileName = $"{testName[(int)AssertActArrangeParts.Scenario]}";
				canParse = true;
			}

			return canParse;
		}
	}
}
