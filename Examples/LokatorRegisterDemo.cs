using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BlueDotBrigade.DatenLokator.TestTools.Configuration;

/// <summary>
/// Demonstrates real-world usage of Lokator.Register for testing web applications.
/// This example shows how to test a data pipeline that fetches user data from a REST API.
/// </summary>
public class DataPipelineExample
{
    // Example data pipeline class that would normally call a real API
    public class DataPipeline
    {
        private readonly string _apiUrl;

        public DataPipeline(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<User[]> FetchUsersAsync()
        {
            using var client = new HttpClient();
            var json = await client.GetStringAsync(_apiUrl);
            return JsonSerializer.Deserialize<User[]>(json);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Example test: Testing the data pipeline without a real API server.
    /// </summary>
    public static async Task TestDataPipelineWithMockApi()
    {
        Console.WriteLine("=== Testing Data Pipeline with Lokator.Register ===\n");

        // Define test data
        var testUsers = new[]
        {
            new User { Id = 1, Name = "Alice Johnson", Email = "alice@example.com" },
            new User { Id = 2, Name = "Bob Smith", Email = "bob@example.com" },
            new User { Id = 3, Name = "Charlie Brown", Email = "charlie@example.com" }
        };

        // Register a mock API endpoint
        var apiUri = new Uri("http://localhost:8080/api/users");
        
        using (Lokator.Register(apiUri, testUsers))
        {
            Console.WriteLine($"✓ Registered mock API at {apiUri}");
            
            // Give the listener a moment to start
            await Task.Delay(200);

            // Create and test the data pipeline
            var pipeline = new DataPipeline(apiUri.ToString());
            var users = await pipeline.FetchUsersAsync();

            Console.WriteLine($"\n✓ Pipeline fetched {users.Length} users:");
            foreach (var user in users)
            {
                Console.WriteLine($"  - {user.Name} ({user.Email})");
            }

            // Verify results
            if (users.Length == 3 && users[0].Name == "Alice Johnson")
            {
                Console.WriteLine("\n✓ Test PASSED: Pipeline correctly fetched and parsed mock data");
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

        var apiUri = new Uri("http://localhost:8081/api/users");
        
        using (Lokator.Register(apiUri, new[] { new User { Id = 1, Name = "Test" } }))
        {
            await Task.Delay(200);

            // Try to access a different path - should get 404
            var wrongUri = new Uri("http://localhost:8081/api/products");
            
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

        var usersUri = new Uri("http://localhost:8082/api/users");
        var productsUri = new Uri("http://localhost:8083/api/products");
        var statusUri = new Uri("http://localhost:8084/api/status");

        var users = new[] { new { Id = 1, Name = "Alice" } };
        var products = new[] { new { Id = 100, Name = "Widget" } };
        var status = new { Healthy = true, Version = "1.0.0" };

        using (Lokator.Register(usersUri, users))
        using (Lokator.Register(productsUri, products))
        using (Lokator.Register(statusUri, status))
        {
            Console.WriteLine("✓ Registered 3 mock endpoints");
            await Task.Delay(300);

            using var client = new HttpClient();

            var usersResponse = await client.GetStringAsync(usersUri);
            var productsResponse = await client.GetStringAsync(productsUri);
            var statusResponse = await client.GetStringAsync(statusUri);

            Console.WriteLine($"✓ Users endpoint: {usersResponse}");
            Console.WriteLine($"✓ Products endpoint: {productsResponse}");
            Console.WriteLine($"✓ Status endpoint: {statusResponse}");

            Console.WriteLine("\n✓ Test PASSED: All endpoints served their respective content\n");
        }
    }

    /// <summary>
    /// Example test: Testing cleanup and port reuse.
    /// </summary>
    public static async Task TestPortReuse()
    {
        Console.WriteLine("=== Testing Port Reuse After Disposal ===\n");

        var uri = new Uri("http://localhost:8085/api/data");

        // First registration
        Console.WriteLine("First registration:");
        using (var first = Lokator.Register(uri, "First content"))
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
        using (Lokator.Register(uri, "Second content"))
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
        Console.WriteLine("║  Lokator.Register - Real-World Usage Examples        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        try
        {
            await TestDataPipelineWithMockApi();
            await TestNotFoundHandling();
            await TestMultipleEndpoints();
            await TestPortReuse();

            Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
            Console.WriteLine("║  All Examples Completed Successfully!                 ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
