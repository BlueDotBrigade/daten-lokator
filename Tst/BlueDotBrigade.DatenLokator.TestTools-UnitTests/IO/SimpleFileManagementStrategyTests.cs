namespace BlueDotBrigade.Datenlokator.TestTools.IO
{
	using System.Collections.Generic;
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
					dotNetOutputFolder);

				var expectedPath = Path.Combine(
					tempDirectory,
					@"\ProjectName\Dat",
					SimpleFileManagementStrategy.GlobalDirectoryName);

				Assert.AreEqual(expectedPath, fileManager.GlobalDirectoryPath);
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
			var testEnvironmentSettings = new Dictionary<string, object>
			{
				{ SimpleFileManagementStrategy.BasePathKey, @"C:\UnitTestData"},
			};

			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				testEnvironmentSettings,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			Assert.AreEqual(@"C:\UnitTestData", fileStrategy.BaseDirectoryPath);
		}

		[TestMethod]
		public void BaseDirectoryPath_ConfigurationIsEmpty_ReturnsAssumedPath()
		{
			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var expectedResult = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName",
				SimpleFileManagementStrategy.BaseDirectoryName);

			Assert.AreEqual(expectedResult, fileStrategy.BaseDirectoryPath);
		}

		[TestMethod]
		public void BaseDirectoryPath_ConfigurationMissing_ReturnsAssumedPath()
		{
			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var expectedResult = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName",
				SimpleFileManagementStrategy.BaseDirectoryName);

			Assert.AreEqual(expectedResult, fileStrategy.BaseDirectoryPath);
		}

		[TestMethod]
		public void GraphToBaseDirectory_ConfigurationIsValid_ReturnsConfigurationPlusTest()
		{
			var testEnvironmentSettings = new Dictionary<string, object>
			{
				{SimpleFileManagementStrategy.BasePathKey, @"C:\UnitTestData"},
			};

			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				testEnvironmentSettings,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath =
				fileStrategy.GraphOnToBaseDirectory(
					@"C:\SourceCode\ApplicationName\ProjectName\Diagnostics\DebugTest.cs");

			var expectedPath = Path.Combine(@"C:\UnitTestData\", @"Diagnostics\DebugTest.cs");

			Assert.AreEqual(expectedPath, actualPath);
		}

		[TestMethod]
		public void GraphToBaseDirectory_ConfigurationIsEmpty_ReturnsExecutingAssemblyPlusTest()
		{
			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath =
				fileStrategy.GraphOnToBaseDirectory(
					@"C:\SourceCode\ApplicationName\ProjectName\Diagnostics\DebugTest.cs");

			var expectedPath = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName\",
				SimpleFileManagementStrategy.BaseDirectoryName,
				@"Diagnostics\DebugTest.cs");

			Assert.AreEqual(expectedPath, actualPath);
		}

		[TestMethod]
		public void GraphToBaseDirectory_ConfigurationMissing_ReturnsExecutingAssemblyPlusTest()
		{
			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				new Mock<IOsFile>().Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath =
				fileStrategy.GraphOnToBaseDirectory(
					@"C:\SourceCode\ApplicationName\ProjectName\Diagnostics\DebugTest.cs");

			var expectedPath = Path.Combine(
				@"C:\SourceCode\ApplicationName\ProjectName\",
				SimpleFileManagementStrategy.BaseDirectoryName,
				@"Diagnostics\DebugTest.cs");

			Assert.AreEqual(expectedPath, actualPath);
		}

		[TestMethod]
		public void GetFileOrInferName_ExplicitFileNameAndDirectory_ReturnsRequestedPath()
		{
			var fileTool = new Mock<IOsFile>();

			var localFile = $@"T:\TestData\Data.xml";
			fileTool.Setup(f => f.Exists(localFile)).Returns(true);

			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				fileTool.Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualFileChosen = fileStrategy.GetFileOrInferName("Data.xml", @"T:\TestData");

			Assert.AreEqual(localFile, actualFileChosen);
		}

		[TestMethod]
		public void GetFileOrInferName_ExplicitDirectory_ReturnsImpliedFileWithoutExtension()
		{
			var mockedFileUtility = new Mock<IOsFile>();

			mockedFileUtility.Setup(f =>
					f.Exists(@"T:\TestData\GetFileOrInferName_ExplicitDirectory_ReturnsDirectoryWithImpliedFile"))
				.Returns(false);

			mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario"))
				.Returns(true);

			var mockedDirectoryUtility = new Mock<IOsDirectory>();
			mockedDirectoryUtility.Setup(d => d.GetFiles("TestedScenario"))
				.Returns(new[] { @"T:\TestData\TestedScenario.xml" });

			var fileStrategy = new SimpleFileManagementStrategy(
				mockedDirectoryUtility.Object,
				mockedFileUtility.Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath = fileStrategy.GetFileOrInferName(fileName: "MethodName_TestedScenario_Result",
				directoryPath: @"T:\TestData");

			Assert.AreEqual(@"T:\TestData\TestedScenario", actualPath);
		}

		[TestMethod]
		public void GetFileOrInferName_ExplicitDirectory_ReturnsDirectoryWithImpliedFile()
		{
			var mockedFileUtility = new Mock<IOsFile>();

			mockedFileUtility.Setup(f =>
					f.Exists(@"T:\TestData\GetFileOrInferName_ExplicitDirectory_ReturnsDirectoryWithImpliedFile"))
				.Returns(false);

			mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario"))
				.Returns(false);

			mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario.xml"))
				.Returns(true);

			var mockedDirectoryUtility = new Mock<IOsDirectory>();
			mockedDirectoryUtility
				.Setup(d => d.GetFiles(@"T:\TestData", "TestedScenario.*", SearchOption.TopDirectoryOnly))
				.Returns(new[] { new FileInfo(@"T:\TestData\TestedScenario.xml") });

			var fileStrategy = new SimpleFileManagementStrategy(
				mockedDirectoryUtility.Object,
				mockedFileUtility.Object,
				@"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

			var actualPath = fileStrategy.GetFileOrInferName(fileName: "MethodName_TestedScenario_Result",
				directoryPath: @"T:\TestData");

			Assert.AreEqual(@"T:\TestData\TestedScenario.xml", actualPath);
		}

		[TestMethod]
		public void GetFileOrInferName_LocalFileExists_ReturnsLocalFile()
		{
			var fileTool = new Mock<IOsFile>();

			var localFile = $@"T:\TestData\Data.xml";
			fileTool.Setup(f => f.Exists(localFile)).Returns(true);

			var globalFile = $@"C:\ProjectName\Dat\{SimpleFileManagementStrategy.GlobalDirectoryName}\Data.xml";
			fileTool.Setup(f => f.Exists(globalFile)).Returns(true);

			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				fileTool.Object,
				@"C:\ProjectName\bin\x64\Debug");

			var actualFileChosen = fileStrategy.GetFileOrInferName("Data.xml", @"T:\TestData");

			Assert.AreEqual(localFile, actualFileChosen);
		}

		[TestMethod]
		public void GetFileOrInferName_LocalFileMissing_ReturnsGlobalFile()
		{
			var fileTool = new Mock<IOsFile>();

			var localFile = $@"T:\TestData\Data.xml";
			fileTool.Setup(f => f.Exists(localFile)).Returns(false);

			var globalFile = $@"C:\ProjectName\Dat\{SimpleFileManagementStrategy.GlobalDirectoryName}\Data.xml";
			fileTool.Setup(f => f.Exists(globalFile)).Returns(true);

			var fileStrategy = new SimpleFileManagementStrategy(
				new Mock<IOsDirectory>().Object,
				fileTool.Object,
				@"C:\ProjectName\bin\x64\Debug");

			var actualFileChosen = fileStrategy.GetFileOrInferName("Data.xml", @"T:\TestData");

			Assert.AreEqual(globalFile, actualFileChosen);
		}
	}
}