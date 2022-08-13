namespace BlueDotBrigade.Datenlokator.TestTools.Configuration
{
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.Strategies;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;

	/// <summary>
	/// These test methods are more about exploring the <see cref="Lokator"/> API, to ensure that the library can be extended.
	/// </summary>
	[TestClass]
	internal class LokatorShould
	{

		[TestMethod]
		public void AcceptGlobalDefaultFileName()
		{
			new Lokator().UsingDefaultFile("");

			Assert.Fail("incomplete test");
		}
	}
}
