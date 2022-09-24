namespace BlueDotBrigade.DatenLokator.Demo.Serialization
{
	using System.IO;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml;

	internal class XmlSerializer
	{
		internal string ToXml(object value)
		{
			var settings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t",
				NamespaceHandling = NamespaceHandling.OmitDuplicates,
			};

			var result = string.Empty;

			using (var stream = new MemoryStream())
			{
				using (var xmlWriter = XmlWriter.Create(stream, settings))
				{
					var serializer = new DataContractSerializer(value.GetType());
					serializer.WriteObject(xmlWriter, value);
				}

				stream.Position = 0;

				using (var streamRead = new StreamReader(stream))
				{
					result = streamRead.ReadToEnd();
				}

				stream.Close();
			}

			return result;
		}
	}
}
