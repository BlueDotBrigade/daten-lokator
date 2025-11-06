namespace BlueDotBrigade.DatenLokator.TestTools.Uri
{
	using System;
	using System.Net.Http;
	using System.Text.Json;
	using System.Threading.Tasks;
	using BlueDotBrigade.DatenLokator.TestTools.Configuration;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class LokatorRegisterWithDatenShould
	{
		public class User
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		[TestMethod]
		public async Task RegisterUri_WithDatenInstance_ServesContent()
		{
			// Arrange
			var uri = new Uri("http://localhost:5058/users");

			// Act
			using (Lokator.Register(uri, new Daten()))
			{
				using var client = new HttpClient();
				var response = await client.GetStringAsync(uri);
				var users = JsonSerializer.Deserialize<User[]>(response);

				// Assert
				Assert.IsNotNull(users);
				Assert.AreEqual(2, users.Length);
				Assert.AreEqual("Alice", users[0].Name);
				Assert.AreEqual("Bob", users[1].Name);
			}
		}

		[TestMethod]
		public void RegisterUri_WithNullDaten_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				// Act
				Lokator.Register(new Uri("http://localhost:5059/test"), (Daten)null);
				return (object?)null;
			});
		}
	}
}
