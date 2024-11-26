namespace BlueDotBrigade.DatenLokator.TestTools.Configuration
{
	using BlueDotBrigade.DatenLokator.TestTools.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using NSubstitute;

	/// <summary>
	/// These methods are more about exploring the <see cref="Lokator"/> API, to ensure that the library can be extended.
	/// </summary>
	[TestClass]
	public class LokatorApi
	{
		[TestMethod]
		public void UsingFileManagementStrategy()
		{
			var fakeFileManagementStrategy = Substitute.For<IFileManagementStrategy>();

			new Lokator().UsingFileManagementStrategy(fakeFileManagementStrategy);
		}

		[TestMethod]
		public void UsingTestNamingStrategy()
		{
			var fakeTestNamingStrategy = Substitute.For<ITestNamingStrategy>();

			new Lokator().UsingTestNamingStrategy(fakeTestNamingStrategy);
		}
	}
}
