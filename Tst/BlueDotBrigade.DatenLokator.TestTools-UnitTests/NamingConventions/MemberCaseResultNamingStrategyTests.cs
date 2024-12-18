﻿namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class MemberCaseResultNamingStrategyTests
	{
		[TestMethod]
		public void TryGetFileName_ValidInput_ReturnsTrue()
		{
			var strategy = new MemberCaseResultNamingStrategy();
			var wasSuccessful = strategy.TryGetFileName("MethodName_UseCase_ExpectedResult", out var _);

			Assert.IsTrue(wasSuccessful);
		}

		[TestMethod]
		public void TryGetFileName_ValidInput_ReturnsFileName()
		{
			var strategy = new MemberCaseResultNamingStrategy();
			strategy.TryGetFileName("MethodName_UseCase_ExpectedResult", out var fileName);

			Assert.AreEqual("UseCase", fileName);
		}
	}
}