namespace BlueDotBrigade.DatenLokator.Demo.Serialization
{
	using BlueDotBrigade.DatenLokator.Demo.Droids;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class XmlSerializerTests
	{
		/// <summary>
		/// An example of a `traditional` unit test that includes
		/// an embedded string value for comparison.
		/// </summary>
		/// <remarks>
		/// Notice how the...
		/// - incremental visual noise introduced by the multiline string value
		/// - escape codes make the multiline string value harder to read
		/// </remarks>
		[TestMethod]
		public void Serialize_AstromechDroid_StringsMatch()
		{
			// Arrange
			var droid = new AstromechDroid
			{
				SerialNo = "IA-16CFR2D2",
				Manufacturer = "Industrial Automaton",
				ComputerInterface = "SCOMP Link"
			};

			// Act
			var message = new XmlSerializer().ToXml(droid);

			// Assert
			var expectedMessage =
				@"<?xml version=""1.0"" encoding=""utf-8""?>" + "\r\n" +
				@"<AstromechDroid xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/BlueDotBrigade.DatenLokator.Demo.Droids"">" + "\r\n" +
				@"	<Manufacturer>Industrial Automaton</Manufacturer>" + "\r\n" +
				@"	<SerialNo>IA-16CFR2D2</SerialNo>" + "\r\n" +
				@"	<ComputerInterface>SCOMP Link</ComputerInterface>" + "\r\n" +
				@"</AstromechDroid>";

			Assert.AreEqual(expectedMessage, message);
		}

		/// <summary>
		/// An example of DatenLokator automatically retrieving input data
		/// from the `local` test case data.
		/// </summary>
		[TestMethod]
		public void Serialize_ProtocolDroid_StringsMatch()
		{
			// Arrange
			var droid = new ProtocolDroid
			{
				SerialNo = "CG-C3P09D41C",
				Manufacturer = "Cybot Galactica",
				SupportedLanguages = 6_000_000
			};

			// Act
			var message = new XmlSerializer().ToXml(droid);

			// Assert
			Assert.AreEqual(new Daten().AsString(), message);
		}

		/// <summary>
		/// An example of how DatenLokator retrieving input data
		/// using an explicit file name.
		/// </summary>
		[TestMethod]
		public void Serialize_RudeProtocolDroid_StringsMatch()
		{
			// Arrange
			var droid = new ProtocolDroid
			{
				SerialNo = "CG-E3PO9D41C",
				Manufacturer = "Cybot Galactica",
				SupportedLanguages = 1_235_813
			};

			// Act
			var message = new XmlSerializer().ToXml(droid);

			// Assert
			Assert.AreEqual(new Daten().AsString("Bespin.xml"), message);
		}

		/// <summary>
		/// An example of DatenLokator automatically retrieving input data
		/// from the `global` test case data.
		/// </summary>
		/// <remarks>
		/// This approach is useful when many automated tests rely on
		/// the same input data.
		/// </remarks>
		[TestMethod]
		public void Serialize_PitDroid_StringsMatch()
		{
			// Arrange
			var droid = new PitDroid
			{
				SerialNo = "SD-922WAC471",
				Manufacturer = "Serv-O-Droid",
				Series = "DUM",
			};

			// Act
			var message = new XmlSerializer().ToXml(droid);

			// Assert
			Assert.AreEqual(new Daten().AsString(), message);
		}
	}
}
