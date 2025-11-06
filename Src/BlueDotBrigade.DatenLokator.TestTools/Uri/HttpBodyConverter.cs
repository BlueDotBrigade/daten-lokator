namespace BlueDotBrigade.DatenLokator.TestTools.Uri
{
	using System;
	using System.IO;
	using System.Text;
	using System.Text.Json;

	/// <summary>
	/// Converts various types of test data into HTTP-compatible byte arrays.
	/// </summary>
	internal static class HttpBodyConverter
	{
		/// <summary>
		/// Converts the given content to a byte array suitable for HTTP response.
		/// </summary>
		/// <param name="content">The content to convert. Can be string, byte[], Stream, or any serializable object.</param>
		/// <param name="contentType">Optional content type. If not provided, will be inferred from content.</param>
		/// <returns>A tuple containing the byte array and the content type.</returns>
		public static (byte[] Bytes, string ContentType) ToBytes(object content, string contentType = null)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			byte[] bytes;
			string inferredContentType = contentType;

			switch (content)
			{
				case string stringContent:
					bytes = Encoding.UTF8.GetBytes(stringContent);
					inferredContentType ??= "text/plain; charset=utf-8";
					break;

				case byte[] byteArray:
					bytes = byteArray;
					inferredContentType ??= "application/octet-stream";
					break;

				case Stream stream:
					using (var memoryStream = new MemoryStream())
					{
						stream.CopyTo(memoryStream);
						bytes = memoryStream.ToArray();
					}
					inferredContentType ??= "application/octet-stream";
					break;

				default:
					// Serialize as JSON for any other object type
					var json = JsonSerializer.Serialize(content);
					bytes = Encoding.UTF8.GetBytes(json);
					inferredContentType ??= "application/json; charset=utf-8";
					break;
			}

			return (bytes, inferredContentType);
		}
	}
}
