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
	internal class LokatorTests
	{
		[TestMethod]
		public void UsingFileManagementStrategy_NewImplemenation_NoException()
		{
			var fakeFileManagementStrategy = new Mock<IFileManagementStrategy>().Object;

			new Lokator().UsingFileManagementStrategy(fakeFileManagementStrategy);
		}

		[TestMethod]
		public void UsingTestNamingStrategy_NewImplemenation_NoException()
		{
			var fakeTestNamingStrategy = new Mock<ITestNamingStrategy>().Object;

			new Lokator().UsingTestNamingStrategy(fakeTestNamingStrategy);
		}
	}
}
