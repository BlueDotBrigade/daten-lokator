namespace Demo.Tests
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using BlueDotBrigade.DatenLokator.TestTools.Configuration;

	using Demo.Droids;

	[TestClass]
    public class DroidInventoryPipelineTests
    {
        [TestMethod]
        public async Task FetchDroidsAsync_WithMockApi_ReturnsExpectedDroids()
        {
            // Arrange
            var testDroids = new[]
            {
                new AstromechDroid { SerialNo = "R2-D2", Manufacturer = "Industrial Automaton", ProductLine = "R2-series" },
                new AstromechDroid { SerialNo = "R2-KT", Manufacturer = "Industrial Automaton", ProductLine = "R2-series" },
                new AstromechDroid { SerialNo = "BB-8", Manufacturer = "Industrial Automaton", ProductLine = "BB-series" }
            };

            var apiUri = new Uri("http://localhost:8080/api/droids");

            // Act / Assert
            using (Lokator.Register(apiUri, testDroids))
            {
                // give the in-process listener time to start
                await Task.Delay(200);

                var pipeline = new DroidInventoryPipeline(apiUri.ToString());
                var droids = await pipeline.FetchDroidsAsync();

                Assert.IsNotNull(droids, "Expected non-null droid array");
                Assert.AreEqual(3, droids.Length, "Expected three droids returned");
                Assert.AreEqual("R2-D2", droids[0].SerialNo, "Expected first droid to be R2-D2");
            }
        }

        [TestMethod]
        public async Task Register_UnregisteredPath_ReturnsNotFound()
        {
            // Arrange
            var apiUri = new Uri("http://localhost:8081/api/droids");

            using (Lokator.Register(apiUri, new[] { new AstromechDroid { SerialNo = "R5-D4", Manufacturer = "Industrial Automaton", ProductLine = "R5-series" } }))
            {
                await Task.Delay(200);

                // Try to access a different path - should get 404
                var wrongUri = new Uri("http://localhost:8081/api/starships");

                using var client = new HttpClient();
                var response = await client.GetAsync(wrongUri);

                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Expected 404 for an unregistered path");
            }
        }

        [TestMethod]
        public async Task Register_MultipleEndpoints_ServeDistinctContent()
        {
            // Arrange
            var droidsUri = new Uri("http://localhost:8082/api/droids");
            var peripheralsUri = new Uri("http://localhost:8083/api/peripherals");
            var hangarUri = new Uri("http://localhost:8084/api/hangar-status");

            var droids = new[] { new { SerialNo = "C-3PO", Type = "Protocol Droid" } };
            var peripherals = new[] { new { Type = "Holographic Projector", SerialNumber = "HP-2187" } };
            var hangarStatus = new { Operational = true, BayNumber = "327" };

            using (Lokator.Register(droidsUri, droids))
            using (Lokator.Register(peripheralsUri, peripherals))
            using (Lokator.Register(hangarUri, hangarStatus))
            {
                await Task.Delay(300);

                using var client = new HttpClient();

                var droidsResponse = await client.GetStringAsync(droidsUri);
                var peripheralsResponse = await client.GetStringAsync(peripheralsUri);
                var hangarResponse = await client.GetStringAsync(hangarUri);

                Assert.IsTrue(droidsResponse.Contains("C-3PO"), "Droids endpoint should contain C-3PO");
                Assert.IsTrue(peripheralsResponse.Contains("HP-2187"), "Peripherals endpoint should contain HP-2187");
                Assert.IsTrue(hangarResponse.Contains("327") || hangarResponse.Contains("BayNumber"), "Hangar status endpoint should contain bay information");
            }
        }

        [TestMethod]
        public async Task Register_PortReuse_AfterDisposal_AllowsReRegistration()
        {
            // Arrange
            var uri = new Uri("http://localhost:8085/api/droid-info");

            // First registration
            using (Lokator.Register(uri, "R2-D2 diagnostic data"))
            {
                await Task.Delay(200);
                using var client = new HttpClient();
                var response = await client.GetStringAsync(uri);
                Assert.AreEqual("R2-D2 diagnostic data", response, "First registration should serve the first payload");
            }

            // Ensure the listener cleaned up
            await Task.Delay(500);

            // Second registration on same port
            using (Lokator.Register(uri, "BB-8 diagnostic data"))
            {
                await Task.Delay(200);
                using var client = new HttpClient();
                var response = await client.GetStringAsync(uri);
                Assert.AreEqual("BB-8 diagnostic data", response, "Second registration should serve the second payload");
            }
        }
    }
}
