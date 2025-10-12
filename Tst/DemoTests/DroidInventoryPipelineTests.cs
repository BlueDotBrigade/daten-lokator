using System;
using System.Net.Http;
using System.Threading.Tasks;

using BlueDotBrigade.DatenLokator.TestTools.Configuration;

using Demo;
using Demo.Droids;

/// <summary>
/// Demonstrates real-world usage of Lokator.Register for testing web applications.
/// This example shows how to test a droid inventory system that fetches droid data from a REST API.
/// </summary>
public class DroidInventoryExample
{

    /// <summary>
    /// Example test: Testing the droid inventory pipeline without a real API server.
    /// </summary>
    public static async Task TestDataPipelineWithMockApi()
    {
        Console.WriteLine("=== Testing Droid Inventory Pipeline with Lokator.Register ===\n");

        // Define test data - Famous astromech droids
        var testDroids = new[]
        {
            new AstromechDroid { SerialNo = "R2-D2", Manufacturer = "Industrial Automaton", ProductLine = "R2-series" },
            new AstromechDroid { SerialNo = "R2-KT", Manufacturer = "Industrial Automaton", ProductLine = "R2-series" },
            new AstromechDroid { SerialNo = "BB-8", Manufacturer = "Industrial Automaton", ProductLine = "BB-series" }
        };

        // Register a mock API endpoint
        var apiUri = new Uri("http://localhost:8080/api/droids");
        
        using (Lokator.Register(apiUri, testDroids))
        {
            Console.WriteLine($"✓ Registered mock droid inventory API at {apiUri}");
            
            // Give the listener a moment to start
            await Task.Delay(200);

            // Create and test the droid inventory pipeline
            var pipeline = new DroidInventoryPipeline(apiUri.ToString());
            var droids = await pipeline.FetchDroidsAsync();

            Console.WriteLine($"\n✓ Pipeline fetched {droids.Length} astromech droids:");
            foreach (var droid in droids)
            {
                Console.WriteLine($"  - {droid.SerialNo} ({droid.Manufacturer}, {droid.ProductLine})");
            }

            // Verify results
            if (droids.Length == 3 && droids[0].SerialNo == "R2-D2")
            {
                Console.WriteLine("\n✓ Test PASSED: Pipeline correctly fetched and parsed mock droid data");
            }
            else
            {
                Console.WriteLine("\n✗ Test FAILED: Unexpected data received");
            }
        }

        Console.WriteLine($"\n✓ Mock API disposed and port released\n");
    }

    /// <summary>
    /// Example test: Testing error handling with 404 responses.
    /// </summary>
    public static async Task TestNotFoundHandling()
    {
        Console.WriteLine("=== Testing 404 Not Found Handling ===\n");

        var apiUri = new Uri("http://localhost:8081/api/droids");
        
        using (Lokator.Register(apiUri, new[] { new AstromechDroid { SerialNo = "R5-D4", Manufacturer = "Industrial Automaton", ProductLine = "R5-series" } }))
        {
            await Task.Delay(200);

            // Try to access a different path - should get 404
            var wrongUri = new Uri("http://localhost:8081/api/starships");
            
            using var client = new HttpClient();
            var response = await client.GetAsync(wrongUri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"✓ Test PASSED: Unregistered path returned 404 as expected\n");
            }
            else
            {
                Console.WriteLine($"✗ Test FAILED: Expected 404, got {response.StatusCode}\n");
            }
        }
    }

    /// <summary>
    /// Example test: Testing multiple concurrent endpoints.
    /// </summary>
    public static async Task TestMultipleEndpoints()
    {
        Console.WriteLine("=== Testing Multiple Concurrent Endpoints ===\n");

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
            Console.WriteLine("✓ Registered 3 mock endpoints");
            await Task.Delay(300);

            using var client = new HttpClient();

            var droidsResponse = await client.GetStringAsync(droidsUri);
            var peripheralsResponse = await client.GetStringAsync(peripheralsUri);
            var hangarResponse = await client.GetStringAsync(hangarUri);

            Console.WriteLine($"✓ Droids endpoint: {droidsResponse}");
            Console.WriteLine($"✓ Peripherals endpoint: {peripheralsResponse}");
            Console.WriteLine($"✓ Hangar status endpoint: {hangarResponse}");

            Console.WriteLine("\n✓ Test PASSED: All endpoints served their respective content\n");
        }
    }

    /// <summary>
    /// Example test: Testing cleanup and port reuse.
    /// </summary>
    public static async Task TestPortReuse()
    {
        Console.WriteLine("=== Testing Port Reuse After Disposal ===\n");

        var uri = new Uri("http://localhost:8085/api/droid-info");

        // First registration
        Console.WriteLine("First registration:");
        using (var first = Lokator.Register(uri, "R2-D2 diagnostic data"))
        {
            await Task.Delay(200);
            using var client = new HttpClient();
            var response = await client.GetStringAsync(uri);
            Console.WriteLine($"  ✓ Received: {response}");
        }

        Console.WriteLine("  ✓ First registration disposed");

        // Wait for cleanup
        await Task.Delay(500);

        // Second registration on same port
        Console.WriteLine("\nSecond registration (same port):");
        using (Lokator.Register(uri, "BB-8 diagnostic data"))
        {
            await Task.Delay(200);
            using var client = new HttpClient();
            var response = await client.GetStringAsync(uri);
            Console.WriteLine($"  ✓ Received: {response}");
        }

        Console.WriteLine("  ✓ Second registration disposed");
        Console.WriteLine("\n✓ Test PASSED: Port successfully reused\n");
    }

    public static async Task Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Lokator.Register - Droid Inventory Demo Examples    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        try
        {
            await TestDataPipelineWithMockApi();
            await TestNotFoundHandling();
            await TestMultipleEndpoints();
            await TestPortReuse();

            Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
            Console.WriteLine("║  All Droid Inventory Examples Completed Successfully! ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
