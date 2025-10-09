namespace BlueDotBrigade.DatenLokator.TestTools.Uri
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents a URI registration that serves test data via an HTTP listener.
	/// </summary>
	internal sealed class UrlRegistration : IDisposable
	{
		private readonly HttpListener _listener;
		private readonly System.Uri _uri;
		private readonly byte[] _contentBytes;
		private readonly string _contentType;
		private readonly int _statusCode;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly Task _listenerTask;
		private bool _disposed;

		/// <summary>
		/// Creates a new URL registration that serves the specified content.
		/// </summary>
		/// <param name="uri">The URI to register.</param>
		/// <param name="content">The content to serve.</param>
		/// <param name="contentType">The content type (optional, will be inferred if not provided).</param>
		/// <param name="statusCode">The HTTP status code to return (default: 200).</param>
		public UrlRegistration(System.Uri uri, object content, string contentType = null, int statusCode = 200)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentException("URI must be absolute.", nameof(uri));
			}

			if (uri.Scheme != "http" && uri.Scheme != "https")
			{
				throw new ArgumentException("URI must use HTTP or HTTPS scheme.", nameof(uri));
			}

			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			_uri = uri;
			_statusCode = statusCode;

			// Convert content to bytes
			var (bytes, inferredContentType) = HttpBodyConverter.ToBytes(content, contentType);
			_contentBytes = bytes;
			_contentType = inferredContentType;

			// Create and start HTTP listener
			_listener = new HttpListener();
			
			// Build the prefix from the URI
			var prefix = $"{uri.Scheme}://{uri.Host}:{uri.Port}{uri.AbsolutePath}";
			if (!prefix.EndsWith("/"))
			{
				prefix += "/";
			}
			
			_listener.Prefixes.Add(prefix);
			
			try
			{
				_listener.Start();
			}
			catch (HttpListenerException ex)
			{
				throw new InvalidOperationException(
					$"Failed to start HTTP listener on {uri}. This may be due to port conflicts or permission issues.", ex);
			}

			// Start listening for requests
			_cancellationTokenSource = new CancellationTokenSource();
			_listenerTask = Task.Run(() => ListenAsync(_cancellationTokenSource.Token));
		}

		private async Task ListenAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested && _listener.IsListening)
			{
				try
				{
					var context = await _listener.GetContextAsync();
					await HandleRequestAsync(context);
				}
				catch (HttpListenerException)
				{
					// Listener was stopped
					break;
				}
				catch (ObjectDisposedException)
				{
					// Listener was disposed
					break;
				}
			}
		}

		private async Task HandleRequestAsync(HttpListenerContext context)
		{
			var request = context.Request;
			var response = context.Response;

			try
			{
				// Check if the requested path matches our registered URI
				var requestedPath = request.Url.AbsolutePath;
				var registeredPath = _uri.AbsolutePath;

				if (requestedPath.TrimEnd('/') == registeredPath.TrimEnd('/'))
				{
					// Serve the registered content
					response.StatusCode = _statusCode;
					response.ContentType = _contentType;
					response.ContentLength64 = _contentBytes.Length;

					await response.OutputStream.WriteAsync(_contentBytes, 0, _contentBytes.Length);
				}
				else
				{
					// Path not found
					response.StatusCode = (int)HttpStatusCode.NotFound;
					var notFoundMessage = System.Text.Encoding.UTF8.GetBytes("404 Not Found");
					response.ContentLength64 = notFoundMessage.Length;
					await response.OutputStream.WriteAsync(notFoundMessage, 0, notFoundMessage.Length);
				}
			}
			catch (Exception)
			{
				// Ignore exceptions during request handling
			}
			finally
			{
				response.Close();
			}
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			// Stop listening
			_cancellationTokenSource?.Cancel();
			
			try
			{
				_listener?.Stop();
				_listener?.Close();
			}
			catch
			{
				// Ignore exceptions during cleanup
			}

			// Wait for the listener task to complete (with timeout)
			try
			{
				_listenerTask?.Wait(TimeSpan.FromSeconds(5));
			}
			catch
			{
				// Ignore exceptions during cleanup
			}

			_cancellationTokenSource?.Dispose();
		}
	}
}
