namespace BlueDotBrigade.DatenLokator.TestTools.Uri
{
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using BlueDotBrigade.DatenLokator.TestTools.Configuration;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class LokatorRegisterShould
	{
		[TestMethod]
		public async Task RegisterUri_WithStringContent_ServesContent()
		{
			// Arrange
			var uri = new Uri("http://localhost:5050/test");
			var content = "Hello, World!";

			// Act
			using (Lokator.Register(uri, content))
			{
				using var client = new HttpClient();
				var response = await client.GetStringAsync(uri);

				// Assert
				Assert.AreEqual(content, response);
			}
		}

		[TestMethod]
		public async Task RegisterUri_WithJsonContent_ServesJsonContent()
		{
			// Arrange
			var uri = new Uri("http://localhost:5051/api/data");
			var content = new { Id =1, Name = "Test" };

			// Act
			using (Lokator.Register(uri, content))
			{
				using var client = new HttpClient();
				var response = await client.GetStringAsync(uri);

				// Assert
				Assert.IsTrue(response.Contains("\"Id\":1"));
				Assert.IsTrue(response.Contains("\"Name\":\"Test\""));
			}
		}

		[TestMethod]
		public async Task RegisterUri_WithContentType_UsesSpecifiedContentType()
		{
			// Arrange
			var uri = new Uri("http://localhost:5052/test");
			var content = "<html><body>Hello</body></html>";
			var contentType = "text/html";

			// Act
			using (Lokator.Register(uri, content, contentType))
			{
				using var client = new HttpClient();
				var response = await client.GetAsync(uri);

				// Assert
				Assert.AreEqual(contentType, response.Content.Headers.ContentType.MediaType);
			}
		}

		[TestMethod]
		public async Task RegisterUri_AfterDisposal_ThrowsException()
		{
			// Arrange
			var uri = new Uri("http://localhost:5053/test");
			var content = "Test content";

			var registration = Lokator.Register(uri, content);
			registration.Dispose();

			// Act & Assert
			using var client = new HttpClient();
			await Assert.ThrowsAsync<HttpRequestException>(() => client.GetStringAsync(uri));
		}

		[TestMethod]
		public async Task RegisterUri_WithUnregisteredPath_Returns404()
		{
			// Arrange
			var uri = new Uri("http://localhost:5054/registered");
			var content = "Test content";

			// Act
			using (Lokator.Register(uri, content))
			{
				using var client = new HttpClient();
				var response = await client.GetAsync(new Uri("http://localhost:5054/unregistered"));

				// Assert
				Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
			}
		}

		[TestMethod]
		public void RegisterUri_WithNullUri_ThrowsArgumentNullException()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => Lokator.Register(null, "content"));
		}

		[TestMethod]
		public void RegisterUri_WithRelativeUri_ThrowsArgumentException()
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => Lokator.Register(new Uri("/relative", UriKind.Relative), "content"));
		}

		[TestMethod]
		public void RegisterUri_WithNullContent_ThrowsArgumentNullException()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => Lokator.Register(new Uri("http://localhost:5055/test"), null));
		}

		[TestMethod]
		public async Task RegisterUri_MultipleRegistrations_AllServeContent()
		{
			// Arrange
			var uri1 = new Uri("http://localhost:5056/test1");
			var uri2 = new Uri("http://localhost:5057/test2");
			var content1 = "Content1";
			var content2 = "Content2";

			// Act
			using (Lokator.Register(uri1, content1))
			using (Lokator.Register(uri2, content2))
			{
				using var client = new HttpClient();
				var response1 = await client.GetStringAsync(uri1);
				var response2 = await client.GetStringAsync(uri2);

				// Assert
				Assert.AreEqual(content1, response1);
				Assert.AreEqual(content2, response2);
			}
		}
	}
}
