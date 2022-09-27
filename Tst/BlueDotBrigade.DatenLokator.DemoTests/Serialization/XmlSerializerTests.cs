namespace BlueDotBrigade.DatenLokator.Demo.Serialization
{
	using BlueDotBrigade.DatenLokator.Demo.Droids;
	using BlueDotBrigade.DatenLokator.TestsTools;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>
	/// The intent of this class is to demonstrate how to retrieve input data using the DatenLokator library.
	/// </summary>
	/// <remarks>
	/// The DatenLokator's default file management strategy assumes that the input data is stored within:
	/// <code>\BlueDotBrigade.DatenLokator.DemoTests\Dat</code>
	///
	/// With respect to this class, the following search pattern is used
	/// <list type="number">
	/// <item>local directory: <code>\Dat\Serialization\XmlSerializerTests\</code></item>
	/// <item>local compressed data: <code>\Dat\Serialization\XmlSerializerTests.zip</code></item>
	/// <item>global directory: <code>\Dat\~Global\</code></item>
	/// </list>
	/// </remarks>
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
		/// An example of DatenLokator automatically retrieving input data for a specific test case.
		/// </summary>
		/// <remarks>
		/// By default, DatenLokator expects the input data to be stored in a directory structure
		/// that mirrors the namespace. For example, in the case of:
		///
		/// \Serialization\XmlSerializerTests\ProtocolDroid.xml
		///
		/// Where
		/// - Serialization : represents the namespace
		/// - XmlSerializerTests : represents the MsTest class name
		/// - ProtocolDroid.xml : represents the test case
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
		/// An example of DatenLokator automatically retrieving shared input data
		/// from the `global` cache.
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
		/// \Dat\Serialization\XmlSerializerTests\
		/// \Dat\Serialization\XmlSerializerTests.zip
		/// </remarks>
		[TestMethod]
		public void Serialize_SuperBattleDroid_StringsMatch()
		{
			// Arrange
			var droid = new SuperBattleDroid()
			{
				SerialNo = "BA-4248964B3",
				Manufacturer = "Baktoid Automata",
				
				// imagine a large data set where it would be useful
				// to compress the input data
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
