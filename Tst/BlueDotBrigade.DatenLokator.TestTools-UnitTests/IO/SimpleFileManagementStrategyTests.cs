namespace BlueDotBrigade.Datenlokator.TestTools.IO
{
	using System.Collections.Specialized;
	using System.IO;
	using BlueDotBrigade.DatenLokator.TestsTools.IO;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;

	[TestClass]
	public class InputDataSelectorTest
	{
		[TestMethod]
		public void Setup_NewGlobalDirectoryPath_ReturnsGlobalDirectoryPath()
		{
			var tempDirectory = Path.GetTempPath();

			var dotNetOutputFolder = Path.Combine(tempDirectory, @"ProjectName\bin\x64\Debug");

			try
			{
				Directory.CreateDirectory(dotNetOutputFolder);

				var fileManager = new SimpleFileManagementStrategy();
				fileManager.Setup(
					new OsDirectory(),
					new OsFile(),
					new NameValueCollection(),
					dotNetOutputFolder);

				var expectedPath = Path.Combine(
					tempDirectory,
					@"\ProjectName\Dat",
					SimpleFileManagementStrategy.SharedDataDirectory);

				Assert.AreEqual(expectedPath, fileManager.SharedDirectoryPath);
			}
			finally
			{
				if (Directory.Exists(dotNetOutputFolder))
				{
					Directory.Delete(dotNetOutputFolder, true);
				}
			}
		}

		[TestMethod]
		public void BaseDirectoryPath_ConfigurationIsValid_ReturnsConfiguredPath()
		{
			var appSettings = new NameValueCollection()
			{
				{ SimpleFileManagementStrategy.BasePathKey, @"C:\UnitTestData"},
			};

			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				appSettings,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			Assert.AreEqual(@"C:\UnitTestData", pathSelector.BaseDirectoryPath);
		}

		[TestMethod]
		public void BaseDirectoryPath_ConfigurationIsEmpty_ReturnsAssumedPath()
		{
			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var expectedResult = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName",
				SimpleFileManagementStrategy.BaseDirectory);

			Assert.AreEqual(expectedResult, pathSelector.BaseDirectoryPath);
		}

		[TestMethod]
		public void BaseDirectoryPath_ConfigurationMissing_ReturnsAssumedPath()
		{
			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var expectedResult = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName",
				InputPathSelector.BaseDirectoryName);

			Assert.AreEqual(expectedResult, pathSelector.BaseDirectoryPath);
		}

		[TestMethod]
		public void GraphToBaseDirectory_ConfigurationIsValid_ReturnsConfigurationPlusTest()
		{
			var appSettings = new NameValueCollection()
			{
				{InputPathSelector.BasePathKey, @"C:\UnitTestData"},
			};

			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				appSettings,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath =
				pathSelector.GraphOnToBaseDirectory(
					@"C:\SourceCode\ApplicationName\ProjectName\Diagnostics\DebugTest.cs");

			var expectedPath = Path.Combine(@"C:\UnitTestData\", @"Diagnostics\DebugTest.cs");

			Assert.AreEqual(expectedPath, actualPath);
		}

		[TestMethod]
		public void GraphToBaseDirectory_ConfigurationIsEmpty_ReturnsExecutingAssemblyPlusTest()
		{
			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath =
				pathSelector.GraphOnToBaseDirectory(
					@"C:\SourceCode\ApplicationName\ProjectName\Diagnostics\DebugTest.cs");

			var expectedPath = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName\",
				InputPathSelector.BaseDirectoryName,
				@"Diagnostics\DebugTest.cs");

			Assert.AreEqual(expectedPath, actualPath);
		}

		[TestMethod]
		public void GraphToBaseDirectory_ConfigurationMissing_ReturnsExecutingAssemblyPlusTest()
		{
			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath =
				pathSelector.GraphOnToBaseDirectory(
					@"C:\SourceCode\ApplicationName\ProjectName\Diagnostics\DebugTest.cs");

			var expectedPath = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName\",
				InputPathSelector.BaseDirectoryName,
				@"Diagnostics\DebugTest.cs");

			Assert.AreEqual(expectedPath, actualPath);
		}

		[TestMethod]
		public void GetFilePathOrInfer_ExplicitFileNameAndDirectory_ReturnsRequestedPath()
		{
			var fileTool = new Mock<IOsFile>();

			var localFile = $@"T:\TestData\Data.xml";
			fileTool.Setup(f => f.Exists(localFile)).Returns(true);

			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				fileTool.Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualFileChosen = pathSelector.GetFilePathOrInfer("Data.xml", @"T:\TestData");

			Assert.AreEqual(localFile, actualFileChosen);
		}

		[TestMethod]
		public void GetFilePathOrInfer_ExplicitDirectory_ReturnsImpliedFileWithoutExtension()
		{
			var mockedFileUtility = new Mock<IOsFile>();

			mockedFileUtility.Setup(f =>
					f.Exists(@"T:\TestData\GetFilePathOrInfer_ExplicitDirectory_ReturnsDirectoryWithImpliedFile"))
				.Returns(false);

			mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario"))
				.Returns(true);

			var mockedDirectoryUtility = new Mock<IOsDirectory>();
			mockedDirectoryUtility.Setup(d => d.GetFiles("TestedScenario"))
				.Returns(new[] { @"T:\TestData\TestedScenario.xml" });

			var pathSelector = new InputPathSelector(
				mockedDirectoryUtility.Object,
				mockedFileUtility.Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath = pathSelector.GetFilePathOrInfer(fileName: "MethodName_TestedScenario_Result",
				directoryPath: @"T:\TestData");

			Assert.AreEqual(@"T:\TestData\TestedScenario", actualPath);
		}

		[TestMethod]
		public void GetFilePathOrInfer_ExplicitDirectory_ReturnsDirectoryWithImpliedFile()
		{
			var mockedFileUtility = new Mock<IOsFile>();

			mockedFileUtility.Setup(f =>
					f.Exists(@"T:\TestData\GetFilePathOrInfer_ExplicitDirectory_ReturnsDirectoryWithImpliedFile"))
				.Returns(false);

			mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario"))
				.Returns(false);

			mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario.xml"))
				.Returns(true);

			var mockedDirectoryUtility = new Mock<IOsDirectory>();
			mockedDirectoryUtility
				.Setup(d => d.GetFiles(@"T:\TestData", "TestedScenario.*", SearchOption.TopDirectoryOnly))
				.Returns(new[] { new FileInfo(@"T:\TestData\TestedScenario.xml") });

			var pathSelector = new InputPathSelector(
				mockedDirectoryUtility.Object,
				mockedFileUtility.Object,
				new NameValueCollection(),
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath = pathSelector.GetFilePathOrInfer(fileName: "MethodName_TestedScenario_Result",
				directoryPath: @"T:\TestData");

			Assert.AreEqual(@"T:\TestData\TestedScenario.xml", actualPath);
		}

		[TestMethod]
		public void GetFilePathOrInfer_LocalFileExists_ReturnsLocalFile()
		{
			var fileTool = new Mock<IOsFile>();

			var localFile = $@"T:\TestData\Data.xml";
			fileTool.Setup(f => f.Exists(localFile)).Returns(true);

			var globalFile = $@"C:\ProjectName\Dat\{InputPathSelector.GlobalDirectory}\Data.xml";
			fileTool.Setup(f => f.Exists(globalFile)).Returns(true);

			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				fileTool.Object,
				new NameValueCollection(),
				@"C:\ProjectName\bin\x64\Debug");

			var actualFileChosen = pathSelector.GetFilePathOrInfer("Data.xml", @"T:\TestData");

			Assert.AreEqual(localFile, actualFileChosen);
		}

		[TestMethod]
		public void GetFilePathOrInfer_LocalFileMissing_ReturnsGlobalFile()
		{
			var fileTool = new Mock<IOsFile>();

			var localFile = $@"T:\TestData\Data.xml";
			fileTool.Setup(f => f.Exists(localFile)).Returns(false);

			var globalFile = $@"C:\ProjectName\Dat\{InputPathSelector.GlobalDirectory}\Data.xml";
			fileTool.Setup(f => f.Exists(globalFile)).Returns(true);

			var pathSelector = new InputPathSelector(
				new Mock<IOsDirectory>().Object,
				fileTool.Object,
				new NameValueCollection(),
				@"C:\ProjectName\bin\x64\Debug");

			var actualFileChosen = pathSelector.GetFilePathOrInfer("Data.xml", @"T:\TestData");

			Assert.AreEqual(globalFile, actualFileChosen);
		}
	}
}