namespace BlueDotBrigade.DatenLokator.Demo
{
	using System;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class TestEnvironment
	{
		/// <summary>
		/// Called once at startup, this method is used to prepare the test environment.
		/// </summary>
		/// <param name="context">Intialized at startup, this data reposistory can be used to store test information.</param>
		/// <remarks>
		/// By default, DatenLokator assumes that the intput data is stored within the project's directory.
		///
		/// A new data directory can be specfied by adding the following key to the <see cref="TestContext"/>:
		/// <example>
		/// context.Properties["DatenLokatorRootPath"] = @"c:\New\Path\Goes\Here";
		/// </example>
		/// </remarks>
		/// <seealso href="https://learn.microsoft.com/en-us/previous-versions/ms404699(v=vs.90)">MSDN: Using the TestContext</seealso>
		/// <seealso href="https://blog.adilakhter.com/2008/05/04/more-on-unit-testing-testcontext/">More on Unit Testing: TestContext</seealso>
		[AssemblyInitialize]
		public static void Setup(TestContext context)
		{
			Console.WriteLine("Test environment is being prepared...");
			Lokator
				.Get()
				.UsingTestContext(context.Properties)
				.Setup();
			Console.WriteLine("Test environment has been prepared.");
		}

		/// <summary>
		/// Called once at shutdown, this method is used to cleanup the test environment.
		/// </summary>
		[AssemblyCleanup]
		public static void Teardown()
		{
			Console.WriteLine("Test environment is being cleaned up...");
			Lokator
				.Get()
				.TearDown();
			Console.WriteLine("Test environment has been cleaned up.");
		}
	}
}
