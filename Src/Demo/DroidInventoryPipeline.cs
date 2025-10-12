namespace Demo
{
	using System.Net.Http;
	using System.Text.Json;
	using System.Threading.Tasks;
	using Demo.Droids;

	// Example droid inventory pipeline class that would normally call a real API
	public class DroidInventoryPipeline
	{
		private readonly string _apiUrl;

		public DroidInventoryPipeline(string apiUrl)
		{
			_apiUrl = apiUrl;
		}

		public async Task<AstromechDroid[]> FetchDroidsAsync()
		{
			using var client = new HttpClient();
			var json = await client.GetStringAsync(_apiUrl);
			return JsonSerializer.Deserialize<AstromechDroid[]>(json);
		}
	}
}
