namespace BlueDotBrigade.DatenLokator.TestsTools.NamingConventions
{
	/// <summary>
	///     In order to automatically retrieve input data, it is assumed
	///     that the unit test will use the following naming convention:
	///     MemberName_Scenario_ExpectedResult
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
	public enum AssertActArrangeParts
	{
		/// <summary>
		/// Represents the property or method that is being tested.
		/// </summary>
		SystemUnderTest = 0,

		/// <summary>
		/// Represents the scenario (i.e. use case) that is being tested.
		/// </summary>
		Scenario = 1,

		/// <summary>
		/// Represents the expected outcome of the test.
		/// </summary>
		ExpectedResult = 2,
	}
}