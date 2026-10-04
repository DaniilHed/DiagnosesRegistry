using Polly;
using Polly.Extensions.Http;
using System.Collections.Concurrent;

namespace DiagnosesRegistry.DiagnosisRegistry
{
	public record ExternalRegistryRequest(Guid DiagnosisId, string Code);
	public record ExternalRegistryResponse(bool IsSuccess, string? ErrorMessage);

	public interface IExternalRegistryClient
	{
		Task<ExternalRegistryResponse> SendAsync(Guid diagnosisId, string code, CancellationToken ct = default);
	}

	public class ExternalRegistryModule : IExternalRegistryClient
	{
		private static readonly ConcurrentDictionary<Guid, int> Attempts = new();
		private readonly HttpClient _http;

		public ExternalRegistryModule(HttpClient http) => _http = http;

		// Send 
		public async Task<ExternalRegistryResponse> SendAsync(Guid id, string code, CancellationToken ct = default)
		{
			try
			{
				var res = await _http.PostAsJsonAsync("/api/external-registry/notify", new ExternalRegistryRequest(id, code), ct);
				if (res.IsSuccessStatusCode) return new(true, null);

				var err = await res.Content.ReadAsStringAsync(ct);
				return new(false, $"[{res.StatusCode}] {err}");
			}
			catch (Exception ex)
			{
				return new(false, $"Connection error: {ex.Message}");
			}
		}

		// Map
		public static void MapEndpoint(IEndpointRouteBuilder app) =>
			app.MapPost("/api/external-registry/notify", (ExternalRegistryRequest? req) =>
			{
				if (req is null || req.DiagnosisId == Guid.Empty || string.IsNullOrWhiteSpace(req.Code))
					return Results.BadRequest("Field Id or code is empty ICD-10.");

				int count = Attempts.AddOrUpdate(req.DiagnosisId, 1, (_, c) => c + 1);

				
				return count < 3
					? Results.StatusCode(StatusCodes.Status503ServiceUnavailable)
					: Results.Accepted(value: new { status = "Accepted", attempts = count });
			});

		// DI register
		public static void Register(IServiceCollection services, string baseAddress)
		{
			services.AddHttpClient<IExternalRegistryClient, ExternalRegistryModule>(c => c.BaseAddress = new Uri(baseAddress))
					.AddPolicyHandler(HttpPolicyExtensions
					.HandleTransientHttpError()
					.WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * retry)));
		}
	}
}
