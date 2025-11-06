# Lokator.Register URI Data Source Feature

## Overview

The `Lokator.Register` feature enables unit and integration tests to simulate network data sources by binding a URI to test data managed by Daten. Lokator transparently handles the HTTP transport and lifecycle, allowing production code that normally performs HTTP/URL calls to operate unchanged in test environments.

## Key Features

- **Declarative**: One line binds a URI to a Daten source
- **Isolated**: Registration is scoped via IDisposable; disposing stops the listener and releases the port
- **Conventional**: Works with library's established file-naming conventions
- **Self-contained**: No external servers or mocks required; everything runs within the test process
- **Compositional**: Works alongside existing file-based Daten usage

## API

### Register with Direct Content

```csharp
public static IDisposable Register(Uri uri, object content, string contentType = null)
```

Creates a temporary in-process HTTP listener that responds to requests for the specified URI using the provided content.

**Parameters:**
- `uri`: The URI to register (must be absolute HTTP/HTTPS)
- `content`: The content to serve (string, byte[], Stream, or any serializable object)
- `contentType`: Optional content type (auto-detected if not provided)

### Register with Daten Instance

```csharp
public static IDisposable Register(Uri uri, Daten daten, string contentType = null)
```

Creates a temporary in-process HTTP listener that responds to requests using data resolved by the provided Daten instance.

**Parameters:**
- `uri`: The URI to register (must be absolute HTTP/HTTPS)
- `daten`: Daten instance that loads data via convention-based `.Daten` inputs
- `contentType`: Optional content type (auto-detected if not provided)

## Usage Examples

### Example 1: Testing with Static JSON Data

```csharp
[Fact]
public async Task GivenPipeline_WhenRegisteredUriUsed_ThenServesDatenContent()
{
    var uri = new Uri("http://localhost:5050/users");
    var users = new[] 
    {
        new { Id = 1, Name = "Alice" },
        new { Id = 2, Name = "Bob" }
    };

    using (Lokator.Register(uri, users))
    {
        var pipeline = new DataPipeline(uri.ToString());
        var result = await pipeline.RunAsync();

        Assert.Equal("Alice", result.First().Name);
    }
}
```

### Example 2: Testing with Daten Convention-Based Data

```csharp
[Fact]
public async Task GivenPipeline_WhenUsingDaten_ThenLoadsFromConventionalPath()
{
    // Assumes .Daten/TestClass/GivenPipeline_WhenUsingDaten_ThenLoadsFromConventionalPath.json exists
    var uri = new Uri("http://localhost:5050/api/users");

    using (Lokator.Register(uri, new Daten()))
    {
        var pipeline = new DataPipeline(uri.ToString());
        var result = await pipeline.RunAsync();

        Assert.NotNull(result);
    }
}
```

### Example 3: Testing with Custom Content Type

```csharp
[Fact]
public async Task GivenXmlEndpoint_WhenRegistered_ThenServesXml()
{
    var uri = new Uri("http://localhost:5050/data.xml");
    var xmlContent = "<users><user>Alice</user></users>";

    using (Lokator.Register(uri, xmlContent, "application/xml"))
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(uri);
        
        Assert.Equal("application/xml", response.Content.Headers.ContentType.MediaType);
    }
}
```

### Example 4: Testing with String Content

```csharp
[Fact]
public async Task GivenTextEndpoint_WhenRegistered_ThenServesText()
{
    var uri = new Uri("http://localhost:5050/readme.txt");
    var content = "This is test documentation.";

    using (Lokator.Register(uri, content))
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(uri);

        Assert.Equal(content, response);
    }
}
```

### Example 5: Testing Multiple Endpoints

```csharp
[Fact]
public async Task GivenMultipleEndpoints_WhenRegistered_ThenAllServeContent()
{
    var usersUri = new Uri("http://localhost:5050/users");
    var productsUri = new Uri("http://localhost:5051/products");

    using (Lokator.Register(usersUri, new { Users = new[] { "Alice", "Bob" } }))
    using (Lokator.Register(productsUri, new { Products = new[] { "Widget", "Gadget" } }))
    {
        using var client = new HttpClient();
        
        var usersResponse = await client.GetStringAsync(usersUri);
        var productsResponse = await client.GetStringAsync(productsUri);

        Assert.Contains("Alice", usersResponse);
        Assert.Contains("Widget", productsResponse);
    }
}
```

## Lifecycle

1. **Register**: Creates HttpListener, binds URI, prepares content
2. **Request Handling**: When the URI is requested, the listener serves the content with correct Content-Type
3. **Dispose**: Listener shuts down and releases all resources/ports automatically

## Content Type Detection

The feature automatically detects content types:

- **string**: `text/plain; charset=utf-8`
- **byte[]**: `application/octet-stream`
- **Stream**: `application/octet-stream`
- **Objects**: Serialized as JSON with `application/json; charset=utf-8`

You can override the content type by providing the `contentType` parameter.

## Error Handling

The feature properly handles error scenarios:

- **Invalid or relative URI**: Throws `ArgumentException`
- **Null URI or content**: Throws `ArgumentNullException`
- **Requests to unregistered paths**: Returns `404 Not Found`
- **Disposed registration**: Connection refused / unavailable
- **Port conflicts**: Throws `InvalidOperationException` with helpful message

## Supported Scenarios

- Testing pipelines or clients that fetch data from remote APIs
- Verifying request routing and content handling without real network dependency
- Running deterministic CI tests that need "HTTP" input semantics
- Simulating various API responses for integration testing
- Testing error handling and retry logic with controlled responses

## Port Reuse

After disposing a registration, the port is properly released and can be reused:

```csharp
[Fact]
public async Task PortCanBeReusedAfterDisposal()
{
    var uri = new Uri("http://localhost:5050/test");

    // First registration
    using (var first = Lokator.Register(uri, "Content 1"))
    {
        // Use the endpoint
    }

    // Port is released, can be reused
    using (var second = Lokator.Register(uri, "Content 2"))
    {
        // Use the endpoint with new content
    }
}
```

## Integration with Existing Features

The URI registration feature works seamlessly with all existing Daten features:

- Convention-based file naming
- Compressed file support
- Global shared files
- Custom file management strategies
- Test naming strategies
- Caching and decompression logic
