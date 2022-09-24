namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	using BlueDotBrigade.DatenLokator.TestsTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class AssertActArrangeTests
	{
		[TestMethod]
		public void TryGetFileName_ValidInput_ReturnsTrue()
		{
			var strategy = new AssertActArrange();
			var wasSuccessful = strategy.TryGetFileName("MethodName_UseCase_ExpectedResult", out var _);

			Assert.IsTrue(wasSuccessful);
		}

		[TestMethod]
		public void TryGetFileName_ValidInput_ReturnsFileName()
		{
			var strategy = new AssertActArrange();
			strategy.TryGetFileName("MethodName_UseCase_ExpectedResult", out var fileName);

			Assert.AreEqual("UseCase", fileName);
		}
	}
}
