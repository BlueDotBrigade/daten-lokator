namespace BlueDotBrigade.DatenLokator.TestTools
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestTools.IO;
	using BlueDotBrigade.DatenLokator.TestTools.NamingConventions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class DatenTests
	{
		private const string DefaultFileName = "DefaultInput.txt";
		private Coordinator _coordinator;

		[TestInitialize]
		public void TestInitialize()
		{
			var osDirectory = new OsDirectory();
			var osFile = new OsFile();

			_coordinator = new Coordinator(
				osFile,
				new MemberCaseResultNamingStrategy(),
				new SubFolderThenGlobal(osDirectory, osFile),
				new Dictionary<string, object>(),
				DefaultFileName);

			_coordinator.Setup();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			// nothing to do
		}

		private byte[] GetBytes(string value)
		{
			var expectedByteArray = new byte[value.Length];

			for (var i = 0; i < value.Length; i++)
			{
				expectedByteArray[i] = Convert.ToByte(value[i]);
			}

			return expectedByteArray;
		}

		#region AsFilePath
		[TestMethod]
		public void AsFilePath_FileByName_ReturnsPath()
		{
			var path = new Daten(_coordinator).AsFilePath("FileByName.txt");

			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsFilePath_FileByConvention_ReturnsPath()
		{
			var path = new Daten(_coordinator).AsFilePath();

			Assert.IsTrue(File.Exists(path));
		}

		[TestMethod]
		public void AsFilePath_FileByDefault_ReturnsPath()
		{
			var path = new Daten(_coordinator).AsFilePath(From.GlobalDefault);

			Assert.IsTrue(File.Exists(path));
			Assert.IsTrue(path.EndsWith(DefaultFileName));
			Assert.IsTrue(path.IndexOf(".Global", StringComparison.Ordinal) > 0);
		}

		#endregion

		#region AsString

		[TestMethod]
		public void AsString_FileByDefault_ReturnsCorrectContent()
		{
			var expected = "Default source file shared by multiple tests globally.";
			var actual = new Daten(_coordinator).AsString(From.GlobalDefault);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void AsString_FileByName_ReturnsCorrectContent()
		{
			Assert.AreEqual(
				"Requested input using specific file name.",
				new Daten(_coordinator).AsString("FileByName.txt"));
		}

		[TestMethod]
		public void AsString_FileByConvention_ReturnsCorrectContent()
		{
			var expected = "Unit test name was used to select a source file.";
			var actual = new Daten(_coordinator).AsString();

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void AsString_MissingFileExtensionByConvention_ReturnsCorrectContent()
		{
			var expected = "Missing file extension still works.";
			var actual = new Daten(_coordinator).AsString();

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void AsString_FileDoesNotExist_ThrowsFileNotFound()
		{
			Assert.Throws<FileNotFoundException>(() =>
			{
				new Daten(_coordinator).AsString("ThisFileCannotBeFound.txt");
				return (object?)null;
			});
		}
		#endregion

		#region AsBytes
		[TestMethod]
		public void AsBytes_FileByDefault_ReturnsCorrectContent()
		{
			var expected = GetBytes("Default source file shared by multiple tests globally.");
			var actual = new Daten(_coordinator).AsBytes(From.GlobalDefault);

			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void AsBytes_FileByName_ReturnsCorrectContent()
		{
			var expected = GetBytes("Requested input using specific file name.");
			var actual = new Daten(_coordinator).AsBytes("FileByName.txt");

			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void AsBytes_FileByConvention_ReturnsCorrectContent()
		{
			var expected = GetBytes("Unit test name was used to select a source file.");
			var actual = new Daten(_coordinator).AsBytes();

			CollectionAssert.AreEqual(expected, actual);
		}
		#endregion

		#region AsStream
		[TestMethod]
		public void AsStream_FileByDefault_ReturnsCorrectContent()
		{
			var expected = "Default source file shared by multiple tests globally.";
			Stream actualStream = new Daten(_coordinator).AsStream(From.GlobalDefault);

			using (var reader = new StreamReader(actualStream))
			{
				Assert.AreEqual(expected, reader.ReadToEnd());
			}
		}

		[TestMethod]
		public void AsStream_FileByName_ReturnsCorrectContent()
		{
			var expected = "Requested input using specific file name.";
			Stream actualStream = new Daten(_coordinator).AsStream("FileByName.txt");

			using (var reader = new StreamReader(actualStream))
			{
				Assert.AreEqual(expected, reader.ReadToEnd());
			}
		}

		[TestMethod]
		public void AsStream_FileByConvention_ReturnsCorrectContent()
		{
			var expected = "Unit test name was used to select a source file.";
			Stream actualStream = new Daten(_coordinator).AsStream();

			using (var reader = new StreamReader(actualStream))
			{
				Assert.AreEqual(expected, reader.ReadToEnd());
			}
		}
		#endregion

		#region AsStreamReader
		[TestMethod]
		public void AsStreamReader_FileByDefault_ReturnsCorrectContent()
		{
			var expected = "Default source file shared by multiple tests globally.";
			StreamReader actual = new Daten(_coordinator).AsStreamReader(From.GlobalDefault);

			Assert.AreEqual(expected, actual.ReadToEnd());
		}

		[TestMethod]
		public void AsStreamReader_FileByName_ReturnsCorrectContent()
		{
			var expected = "Requested input using specific file name.";
			StreamReader actual = new Daten(_coordinator).AsStreamReader("FileByName.txt");

			Assert.AreEqual(expected, actual.ReadToEnd());
		}

		[TestMethod]
		public void AsStreamReader_FileByConvention_ReturnsCorrectContent()
		{
			var expected = "Unit test name was used to select a source file.";
			StreamReader actual = new Daten(_coordinator).AsStreamReader();

			Assert.AreEqual(expected, actual.ReadToEnd());
		}
		#endregion
	}
}
