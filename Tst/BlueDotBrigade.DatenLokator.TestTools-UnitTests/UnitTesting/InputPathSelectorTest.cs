namespace BlueDotBrigade.DatenLokator.TestTools.UnitTesting
{
    using System.Collections.Specialized;
    using System.IO;
    using BlueDotBrigade.DatenLokator.TestsTools.IO;
    using BlueDotBrigade.DatenLokator.TestsTools.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class InputDataSelectorTest
    {
        [TestMethod]
        public void GlobalDirectoryPath_TypicalEnvironment_ReturnsGlobalDirectoryPath()
        {
            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
                new NameValueCollection(),
                @"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

            var expectedPath = Path.Combine(
                @"C:\SourceCode\ApplicationName\ProjectName\Dat",
                InputPathSelector.GlobalDirectory);

            Assert.AreEqual(expectedPath, pathSelector.GlobalDirectoryPath);
        }

        [TestMethod]
        public void BaseDirectoryPath_ConfigurationIsValid_ReturnsConfiguredPath()
        {
            var appSettings = new NameValueCollection()
            {
                {InputPathSelector.BasePathKey, @"C:\UnitTestData"},
            };

            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
                appSettings,
                @"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

            Assert.AreEqual(@"C:\UnitTestData", pathSelector.BaseDirectoryPath);
        }

        [TestMethod]
        public void BaseDirectoryPath_ConfigurationIsEmpty_ReturnsAssumedPath()
        {
            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
                new NameValueCollection(),
                @"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

            var expectedResult = Path.Combine(
                @"C:\SourceCode\ApplicationName\ProjectName",
                InputPathSelector.BaseDirectoryName);

            Assert.AreEqual(expectedResult, pathSelector.BaseDirectoryPath);
        }

        [TestMethod]
        public void BaseDirectoryPath_ConfigurationMissing_ReturnsAssumedPath()
        {
            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
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
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
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
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
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
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object,
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
            var fileTool = new Mock<IFile>();

            var localFile = $@"T:\TestData\Data.xml";
            fileTool.Setup(f => f.Exists(localFile)).Returns(true);

            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                fileTool.Object,
                new NameValueCollection(),
                @"C:\SourceCode\ApplicationName\ProjectName\bin\x64\Debug");

            var actualFileChosen = pathSelector.GetFilePathOrInfer("Data.xml", @"T:\TestData");

            Assert.AreEqual(localFile, actualFileChosen);
        }

        [TestMethod]
        public void GetFilePathOrInfer_ExplicitDirectory_ReturnsImpliedFileWithoutExtension()
        {
            var mockedFileUtility = new Mock<IFile>();

            mockedFileUtility.Setup(f =>
                    f.Exists(@"T:\TestData\GetFilePathOrInfer_ExplicitDirectory_ReturnsDirectoryWithImpliedFile"))
                .Returns(false);

            mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario"))
                .Returns(true);

            var mockedDirectoryUtility = new Mock<IDirectory>();
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
            var mockedFileUtility = new Mock<IFile>();

            mockedFileUtility.Setup(f =>
                    f.Exists(@"T:\TestData\GetFilePathOrInfer_ExplicitDirectory_ReturnsDirectoryWithImpliedFile"))
                .Returns(false);

            mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario"))
                .Returns(false);

            mockedFileUtility.Setup(f => f.Exists(@"T:\TestData\TestedScenario.xml"))
                .Returns(true);

            var mockedDirectoryUtility = new Mock<IDirectory>();
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
            var fileTool = new Mock<IFile>();

            var localFile = $@"T:\TestData\Data.xml";
            fileTool.Setup(f => f.Exists(localFile)).Returns(true);

            var globalFile = $@"C:\ProjectName\Dat\{InputPathSelector.GlobalDirectory}\Data.xml";
            fileTool.Setup(f => f.Exists(globalFile)).Returns(true);

            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                fileTool.Object,
                new NameValueCollection(),
                @"C:\ProjectName\bin\x64\Debug");

            var actualFileChosen = pathSelector.GetFilePathOrInfer("Data.xml", @"T:\TestData");

            Assert.AreEqual(localFile, actualFileChosen);
        }

        [TestMethod]
        public void GetFilePathOrInfer_LocalFileMissing_ReturnsGlobalFile()
        {
            var fileTool = new Mock<IFile>();

            var localFile = $@"T:\TestData\Data.xml";
            fileTool.Setup(f => f.Exists(localFile)).Returns(false);

            var globalFile = $@"C:\ProjectName\Dat\{InputPathSelector.GlobalDirectory}\Data.xml";
            fileTool.Setup(f => f.Exists(globalFile)).Returns(true);

            var pathSelector = new InputPathSelector(
                new Mock<IDirectory>().Object,
                fileTool.Object,
                new NameValueCollection(),
                @"C:\ProjectName\bin\x64\Debug");

            var actualFileChosen = pathSelector.GetFilePathOrInfer("Data.xml", @"T:\TestData");

            Assert.AreEqual(globalFile, actualFileChosen);
        }
    }
}