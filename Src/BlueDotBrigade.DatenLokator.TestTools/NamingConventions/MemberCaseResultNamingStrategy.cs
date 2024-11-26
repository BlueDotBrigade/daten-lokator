namespace BlueDotBrigade.DatenLokator.TestTools.NamingConventions
{
	/// <summary>
	/// Represents a behavior-driven design (BDD) unit test naming convention whereby the method name has three parts:
	/// <list type="number">
	///		<item>member field/property being tested</item>
	///		<item>use case</item>
	///		<item>expected result</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// For example, a unit test might be called:
	/// <list type="bullet">
	///		<item><c>Add_ItemToList_ListContainsItem()</c></item>
	///		<item><c>IsValidEmail_ValidEmailAddress_ReturnsTrue()</c></item>
	///		<item><c>Divide_NonZeroDenominator_ReturnsQuotient()</c></item>
	///		<item><c>OpenFile_FileDoesNotExist_ThrowsFileNotFoundException()</c></item>
	/// </list>
	/// </remarks>
	public class MemberCaseResultNamingStrategy : ITestNamingStrategy
	{
		public bool TryGetFileName(string methodOrFileName, out string fileName)
		{
			var canParse = false;

			fileName = string.Empty;

			var testName = methodOrFileName.Split('_');
			if (testName.Length == 3)
			{
				fileName = $"{testName[(int)MemberCaseResultType.Case]}";
				canParse = true;
			}

			return canParse;
		}
	}
}