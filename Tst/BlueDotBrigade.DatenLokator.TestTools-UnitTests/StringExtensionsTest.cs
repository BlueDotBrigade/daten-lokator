namespace BlueDotBrigade.DatenLokator.TestTools
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using BlueDotBrigade.DatenLokator.TestTools;

    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void LastIndexOfPrefix_NullAndEmptyComparison_ReturnsNoMatch()
        {
            string value1 = null;
            string value2 = string.Empty;

            Assert.IsTrue(value1.LastIndexOfPrefix(value2) < 0);
        }

        [TestMethod]
        public void LastIndexOfPrefix_EmptyAndNullComparison_ReturnsNoMatch()
        {
            string value1 = string.Empty;
            string value2 = null;

            Assert.IsTrue(value2.LastIndexOfPrefix(value1) < 0);
        }

        [TestMethod]
        public void LastIndexOfPrefix_BothEmpty_ReturnsNoMatch()
        {
            var value1 = string.Empty;
            var value2 = string.Empty;

            Assert.IsTrue(value1.LastIndexOfPrefix(value2) < 0);
        }

        [TestMethod]
        public void LastIndexOfPrefix_TwoDifferentStrings_ReturnsNoMatch()
        {
            var value1 = "abc";
            var value2 = "def";

            Assert.IsTrue(value1.LastIndexOfPrefix(value2) < 0);
        }

        [TestMethod]
        public void LastIndexOfPrefix_FirstLetterMatches_Returns0()
        {
            var value1 = "abc";
            var value2 = "aef";

            Assert.AreEqual(0, value1.LastIndexOfPrefix(value2));
        }

        [TestMethod]
        public void LastIndexOfPrefix_FirstTwoLettersMatch_Returns1()
        {
            var value1 = "abc";
            var value2 = "abf";

            Assert.AreEqual(1, value1.LastIndexOfPrefix(value2));
        }

        [TestMethod]
        public void LastIndexOfPrefix_DifferentLengthStrings_ReturnsIndexOfShortString()
        {
            var value1 = "abc";
            var value2 = "abcd";

            Assert.AreEqual(2, value1.LastIndexOfPrefix(value2));
        }
    }
}