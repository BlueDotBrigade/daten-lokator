﻿namespace BlueDotBrigade.DatenLokator.Demo
{
	using System;
	using BlueDotBrigade.DatenLokator.TestsTools.Configuration;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class TestEnvironment
	{
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