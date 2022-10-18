namespace Demo.Serialization
{
	using BlueDotBrigade.DatenLokator.TestsTools;
	using Demo.Droids;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>
	/// The intent of this class is to demonstrate how to retrieve input data using the DatenLokator library.
	/// </summary>
	[TestClass]
	public class XmlSerializerTests
	{
		/// <summary>
		/// An example of a typical unit test that includes an embedded string value for comparison.
		/// </summary>
		/// <remarks>
		/// Notice...
		///  <list type="number">
		///		<item>the visual noise introduced by the multiline string value</item>
		///		<item>how the escape codes make the multiline string value harder to read</item> 
		/// </list>
		/// </remarks>
		[TestMethod]
		public void Serialize_AstromechDroid_StringsMatch()
		{
			// Arrange
			var droid = new AstromechDroid
			{
				SerialNo = "IA-16CFR2D2",
				Manufacturer = "Industrial Automaton",
				ProductLine = "R-Series",
				Peripherals = new Peripheral[]
				{
					new Peripheral { Type = "Mark IV Locomotion System", SerialNumber = "IA-1A963F95"},
					new Peripheral { Type = "SCOMP Link", SerialNumber = "IA-80C56868"},
					new Peripheral { Type = "Fusioncutter", SerialNumber = "IA-6D9D071C"},
					new Peripheral { Type = "Holoprojector", SerialNumber = "IA-12735813"},
				}
			};

			// Act
			var message = new XmlSerializer().ToXml(droid);

			// Assert
			var expectedMessage =
				@"<?xml version=""1.0"" encoding=""utf-8""?>" + "\r\n" +
				@"<AstromechDroid xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Demo.Droids"">" + "\r\n" +
				@"	<Manufacturer>Industrial Automaton</Manufacturer>" + "\r\n" +
				@"	<SerialNo>IA-16CFR2D2</SerialNo>" + "\r\n" +
				@"	<Peripherals>" + "\r\n" +
				@"		<Peripheral>" + "\r\n" +
				@"			<SerialNumber>IA-1A963F95</SerialNumber>" + "\r\n" +
				@"			<Type>Mark IV Locomotion System</Type>" + "\r\n" +
				@"		</Peripheral>" + "\r\n" +
				@"		<Peripheral>" + "\r\n" +
				@"			<SerialNumber>IA-80C56868</SerialNumber>" + "\r\n" +
				@"			<Type>SCOMP Link</Type>" + "\r\n" +
				@"		</Peripheral>" + "\r\n" +
				@"		<Peripheral>" + "\r\n" +
				@"			<SerialNumber>IA-6D9D071C</SerialNumber>" + "\r\n" +
				@"			<Type>Fusioncutter</Type>" + "\r\n" +
				@"		</Peripheral>" + "\r\n" +
				@"		<Peripheral>" + "\r\n" +
				@"			<SerialNumber>IA-12735813</SerialNumber>" + "\r\n" +
				@"			<Type>Holoprojector</Type>" + "\r\n" +
				@"		</Peripheral>" + "\r\n" +
				@"	</Peripherals>" + "\r\n" +
				@"	<ProductLine>R-Series</ProductLine>" + "\r\n" +
				@"</AstromechDroid>";

			// Not only does the preceding line make the test visually noisy,
			// ... but the string is also very error prone.
			Assert.AreEqual(expectedMessage, message);
		}

		/// <summary>
		/// An example of DatenLokator automatically retrieving the correct input data for the test case.
		/// </summary>
		/// <remarks>
		/// By default, DatenLokator expects the input data to be stored in a directory structure
		/// that mirrors the namespace. For example, in this case:
		///
		/// \.Daten\Serialization\XmlSerializerTests\ProtocolDroid.xml
		///
		/// Where:
		/// <list type="bullet">
		///		<item>Serialization : represents the namespace</item>
		///		<item>XmlSerializerTests : represents the MS Test class name</item>
		///		<item>ProtocolDroid.xml : represents the test case</item>
		/// </list>
		/// </remarks>
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
		/// An example of DatenLokator retrieving input data using a specific file name.
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
		/// An example of DatenLokator automatically retrieving shared data
		/// from the `\.Daten\.Global` cache.
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

		/// <summary>
		/// An example of DatenLokator automatically retrieving compressed input data
		/// for a specific test case.
		/// </summary>
		/// <remarks>
		/// DatenLokator expects the zip file to be stored within the directory that
		/// matches the namespace.  In this case:
		///
		/// \.Daten\Serialization\XmlSerializerTests\
		/// \.Daten\Serialization\XmlSerializerTests.zip
		/// </remarks>
		[TestMethod]
		public void Serialize_SuperBattleDroid_StringsMatch()
		{
			// Arrange
			var droid = new SuperBattleDroid()
			{
				SerialNo = "BA-4248964B3",
				Manufacturer = "Baktoid Automata",
				
				// Imagine a large data set where it would be useful
				// to compress the input data.
				WeaponsActivationKey = new byte[8192],
			};

			// Act
			var message = new XmlSerializer().ToXml(droid);

			// Assert
			Assert.AreEqual(
				expected: new Daten().AsString(), 
				actual: message, 
				message:"Input data was not automatically decompressed.");
		}
	}
}
