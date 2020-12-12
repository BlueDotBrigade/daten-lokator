namespace BlueDotBrigade.DatenLokator.TestsTools.UnitTesting
{
    /// <summary>
    ///     In order to automatically retrieve input data, it is assumed
    ///     that the unit test will use the following naming convention:
    ///     MemberName_StateUnderTest_ExpectedBehavior
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestMethod]
    ///     public void Withdraw_FromInvalidAccount_Throws()
    ///     {
    ///         // body of unit test
    ///     }
    /// </code>
    /// </example>
    public enum UnitTestNamingConvention
    {
        /// <summary>
        ///     Represents the member property or method that is being tested.
        /// </summary>
        MemberName = 0,

        /// <summary>
        ///     Represents the scenario (i.e. use case) that is being tested.
        /// </summary>
        StateUnderTest = 1,

        /// <summary>
        ///     Represents the expected outcome of the test.
        /// </summary>
        ExpectedBehavior = 2,
    }
}