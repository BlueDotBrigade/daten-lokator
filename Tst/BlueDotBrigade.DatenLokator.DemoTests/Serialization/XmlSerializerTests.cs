namespace BlueDotBrigade.DatenLokator.Demo.Serialization
{
	using BlueDotBrigade.DatenLokator.Demo.Droids;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class XmlSerializerTests
	{
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
