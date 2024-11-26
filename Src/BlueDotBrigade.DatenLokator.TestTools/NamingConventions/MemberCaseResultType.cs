namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	/// <summary>
	/// In order to automatically retrieve the source data, it is assumed
	/// that the unit test will use the following naming convention:
	/// MemberName_Scenario_ExpectedResult
	/// </summary>
	/// <example>
	///     <code>
	///     [TestMethod]
	///     public void Withdraw_TooMuchMoney_Throws()
	///     {
	///         // body of unit test
	///     }
	/// </code>
	/// </example>
	internal enum MemberCaseResultType
	{
		/// <summary>
		/// Represents the property or method that is being tested.
		/// </summary>
		Member = 0,

		/// <summary>
		/// Represents the use case that is being tested.
		/// </summary>
		Case = 1,

		/// <summary>
		/// Represents the expected outcome of the test.
		/// </summary>
		Result = 2,
	}
}