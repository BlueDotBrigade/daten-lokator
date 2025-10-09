namespace BlueDotBrigade.DatenLokator.TestTools.Uri
{
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using BlueDotBrigade.DatenLokator.TestTools.Configuration;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class LokatorRegisterTests
	{
		[TestMethod]
		public async Task RegisterUri_WithStringContent_ServesContent()
		{
			// Arrange
			var uri = new Uri("http://localhost:6050/test");
			var content = "Hello, World!";

			// Act
			using (Lokator.Register(uri, content))
			{
				// Give the listener a moment to start
				await Task.Delay(100);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
				var response = await client.GetStringAsync(uri);

				// Assert
				Assert.AreEqual(content, response);
			}
		}

		[TestMethod]
		public async Task RegisterUri_WithJsonContent_ServesJsonContent()
		{
			// Arrange
			var uri = new Uri("http://localhost:6051/api/data");
			var content = new { Id = 1, Name = "Test" };

			// Act
			using (Lokator.Register(uri, content))
			{
				await Task.Delay(100);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
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
			var uri = new Uri("http://localhost:6052/test");
			var content = "<html><body>Hello</body></html>";
			var contentType = "text/html";

			// Act
			using (Lokator.Register(uri, content, contentType))
			{
				await Task.Delay(100);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
				var response = await client.GetAsync(uri);

				// Assert
				Assert.AreEqual(contentType, response.Content.Headers.ContentType?.MediaType);
			}
		}

		[TestMethod]
		public async Task RegisterUri_AfterDisposal_ThrowsException()
		{
			// Arrange
			var uri = new Uri("http://localhost:6053/test");
			var content = "Test content";

			var registration = Lokator.Register(uri, content);
			await Task.Delay(100);
			registration.Dispose();
			await Task.Delay(100);

			// Act & Assert
			using var client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(5);
			await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
			{
				await client.GetStringAsync(uri);
			});
		}

		[TestMethod]
		public async Task RegisterUri_WithUnregisteredPath_Returns404()
		{
			// Arrange
			var uri = new Uri("http://localhost:6054/registered");
			var content = "Test content";

			// Act
			using (Lokator.Register(uri, content))
			{
				await Task.Delay(100);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
				var response = await client.GetAsync(new Uri("http://localhost:6054/unregistered"));

				// Assert
				Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RegisterUri_WithNullUri_ThrowsArgumentNullException()
		{
			// Act
			Lokator.Register(null, "content");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterUri_WithRelativeUri_ThrowsArgumentException()
		{
			// Act
			Lokator.Register(new Uri("/relative", UriKind.Relative), "content");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RegisterUri_WithNullContent_ThrowsArgumentNullException()
		{
			// Act
			Lokator.Register(new Uri("http://localhost:6055/test"), null);
		}

		[TestMethod]
		public async Task RegisterUri_MultipleRegistrations_AllServeContent()
		{
			// Arrange
			var uri1 = new Uri("http://localhost:6056/test1");
			var uri2 = new Uri("http://localhost:6057/test2");
			var content1 = "Content 1";
			var content2 = "Content 2";

			// Act
			using (Lokator.Register(uri1, content1))
			using (Lokator.Register(uri2, content2))
			{
				await Task.Delay(200);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
				var response1 = await client.GetStringAsync(uri1);
				var response2 = await client.GetStringAsync(uri2);

				// Assert
				Assert.AreEqual(content1, response1);
				Assert.AreEqual(content2, response2);
			}
		}

		[TestMethod]
		public async Task RegisterUri_CanReusePortAfterDispose()
		{
			// Arrange
			var uri = new Uri("http://localhost:6058/test");
			var content1 = "First registration";
			var content2 = "Second registration";

			// Act - First registration
			using (var registration = Lokator.Register(uri, content1))
			{
				await Task.Delay(100);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
				var response = await client.GetStringAsync(uri);
				Assert.AreEqual(content1, response);
			}

			// Wait for cleanup
			await Task.Delay(500);

			// Act - Second registration on same port
			using (Lokator.Register(uri, content2))
			{
				await Task.Delay(100);
				
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromSeconds(5);
				var response = await client.GetStringAsync(uri);

				// Assert
				Assert.AreEqual(content2, response);
			}
		}
	}
}
