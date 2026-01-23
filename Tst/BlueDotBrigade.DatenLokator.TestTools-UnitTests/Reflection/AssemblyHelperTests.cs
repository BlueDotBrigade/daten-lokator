namespace BlueDotBrigade.DatenLokator.TestTools.Reflection
{
	using System;
	using System.IO;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class AssemblyHelperTests
	{
		private static string NormalizePath(string path)
		{
			return path
				.Replace('\\', Path.DirectorySeparatorChar)
				.Replace('/', Path.DirectorySeparatorChar)
				.TrimEnd(Path.DirectorySeparatorChar);
		}

		[TestMethod]
		public void ExecutingDirectory_MatchesDomainBaseDirectory()
		{
			Assert.AreEqual(
				AppDomain.CurrentDomain.BaseDirectory,
				AssemblyHelper.ExecutingDirectory);
		}

		[TestMethod]
		public void GetProjectDirectoryPath_NoBinSegment_ReturnsTrimmedPath()
		{
			var path = "C:\\Work\\DemoTests\\";

			Assert.AreEqual(
				NormalizePath("C:\\Work\\DemoTests"),
				AssemblyHelper.GetProjectDirectoryPath(path));
		}

		[TestMethod]
		public void GetProjectDirectoryPath_WithBinSegment_ReturnsProjectRoot()
		{
			var path = "C:\\Work\\DemoTests\\bin\\Debug\\";

			Assert.AreEqual(
				NormalizePath("C:\\Work\\DemoTests"),
				AssemblyHelper.GetProjectDirectoryPath(path));
		}
	}
}
