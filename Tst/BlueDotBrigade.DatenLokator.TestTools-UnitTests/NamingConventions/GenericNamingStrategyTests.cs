namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class GenericNamingStrategyTests
	{
		/// <summary>
		/// Reprents a simple implementation of a <see cref="ITestNamingStrategy"/> delegate.
		/// </summary>
		/// <param name="methodName">
		/// The unit test&apos; method name.
		/// </param>
		/// <param name="fileName">
		/// Typically the delegate would return the name of the source file.
		/// In this case, the parser simply returns the provided method name.
		/// </param>
		/// <returns></returns>
		private bool PassThroughParser(string methodName, out string fileName)
		{
			if (string.IsNullOrWhiteSpace(methodName))
			{
				fileName = string.Empty;
				return false;
			}
			else
			{
				fileName = methodName;
				return true;
			}
		}

		[TestMethod]
		public void Ctor_NullMethodName_ReturnsFalse()
		{
			var strategy = new GenericNamingStrategy(PassThroughParser);

			Assert.IsFalse(strategy.TryGetFileName(string.Empty, out _));
		}

		[TestMethod]
		public void Ctor_ValidMethodName_ReturnsTrue()
		{
			var strategy = new GenericNamingStrategy(PassThroughParser);

			Assert.IsTrue(strategy.TryGetFileName("DoWork", out _));
		}

		[TestMethod]
		public void Ctor_ValidMethodName_FileNameSet()
		{
			var strategy = new GenericNamingStrategy(PassThroughParser);

			strategy.TryGetFileName("DoWork", out var fileName);

			Assert.AreEqual("DoWork", fileName);
		}
	}
}
