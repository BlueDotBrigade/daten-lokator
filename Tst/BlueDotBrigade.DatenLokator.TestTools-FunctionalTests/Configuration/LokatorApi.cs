namespace BlueDotBrigade.DatenLokator.TestTools.Configuration
{
	using BlueDotBrigade.DatenLokator.TestTools.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;

	/// <summary>
	/// These methods are more about exploring the <see cref="Lokator"/> API, to ensure that the library can be extended.
	/// </summary>
	[TestClass]
	public class LokatorApi
	{
		[TestMethod]
		public void UsingFileManagementStrategy()
		{
			var fakeFileManagementStrategy = new Mock<IFileManagementStrategy>().Object;

			new Lokator().UsingFileManagementStrategy(fakeFileManagementStrategy);
		}

		public void UsingTestNamingStrategy()
		{
			var fakeTestNamingStrategy = new Mock<ITestNamingStrategy>().Object;

			new Lokator().UsingTestNamingStrategy(fakeTestNamingStrategy);
		}
	}
}
