namespace BlueDotBrigade.DatenLokator.TestTools
{
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using BlueDotBrigade.DatenLokator.TestsTools.Reflection;
    using BlueDotBrigade.DatenLokator.TestsTools.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InputDataTest
    {
        private static string GetProjectDirectory()
        {
            var projectDirectory = Path.Combine(AssemblyHelper.ExecutingDirectory, @"..\..\..\");

            // Remove relative path references (i.e. return a real path)
            return Path.GetFullPath(projectDirectory);
        }

        [TestMethod]
        public void BasePath_AssumedLocalPath_ReturnsPathToProjectFolder()
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["InputDataPath"]),
                $"This test requires that you do NOT define a `{nameof(InputData.BaseDirectoryPath)}` for the input data. BaseDirectoryPath=`{InputData.BaseDirectoryPath}`");

            Debug.WriteLine($"Base path is assumed to be: {InputData.BaseDirectoryPath}");

            var expectedInputDataPath = Path.Combine(GetProjectDirectory(), InputData.BaseDirectoryName);

            Assert.AreEqual(expectedInputDataPath, InputData.BaseDirectoryPath);
        }

        [TestMethod]
        public void BasePath_ExplicitlyDefinedPath_ReturnsPathToProjectFolder()
        {
            var newInputDataFolder = "InputAlternateLocation";
            var originalValue = ConfigurationManager.AppSettings[InputData.BasePathKey];

            try
            {
                ConfigurationManager.AppSettings[InputData.BasePathKey] =
                    Path.Combine(GetProjectDirectory(), newInputDataFolder);
                Debug.WriteLine($"Base path was explicitly set as: {InputData.BaseDirectoryPath}");

                var newBasePath = Path.Combine(GetProjectDirectory(), newInputDataFolder);

                Assert.IsTrue(!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[InputData.BasePathKey]));
                Assert.AreEqual(newBasePath, InputData.BaseDirectoryPath);
            }
            finally
            {
                ConfigurationManager.AppSettings[InputData.BasePathKey] = originalValue;
            }
        }

        [TestMethod]
        public void GetAsString_FileRequestedExplicitly_ReturnsFileContent()
        {
            Assert.AreEqual("NothingWasImplied", InputData.GetAsString("FileRequestedExplicitly.txt"));
        }

        [TestMethod]
        public void GetAsString_FileRequestedImplicitly_ReturnsFileContent()
        {
            Assert.AreEqual("FileNameImpliedByUnitTestName", InputData.GetAsString());
        }

        [TestMethod]
        public void GetAsString_FileRequestedImplicitlyHasExtension_ReturnsFileContent()
        {
            Assert.AreEqual("Convention Over Configuration", InputData.GetAsString());
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetAsString_FileDoesNotExist_ThrowsFileNotFound()
        {
            InputData.GetAsString("FileDoesNotExistOnDisk.txt", @"c:\Invalid\Path\Here");
        }
    }
}