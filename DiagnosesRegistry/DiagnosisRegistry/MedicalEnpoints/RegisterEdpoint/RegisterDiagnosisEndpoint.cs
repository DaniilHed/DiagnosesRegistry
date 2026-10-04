using DiagnosesRegistry.Common;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Interfaces;

namespace DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint
{
	public static class RegisterDiagnosisEndpoint
	{
		public static void MapRegisterDiagnosisEndpoint(this WebApplication app)
		{

			app.MapPost("/api/diagnoses", async (
			RegisterDiagnosisCommand command,
			IRegisterDiagnosisHandler handler,
			CancellationToken ct) =>
			{
				var result = await handler.HandleAsync(command, ct);

				if (!result.IsSuccess)
				{
					return Results.Problem(
						title: "Błąd przetwarzania żądania",
						detail: result.Error.Description,
						statusCode: StatusCodes.Status400BadRequest,
						extensions: new Dictionary<string, object?>
						{
							["errorCode"] = result.Error.Code
						}
					);
				}

				return Results.Created($"/api/diagnoses/{result.Value.Id}", result.Value);
			}).WithName("RegisterDiagnosis")
			.Accepts<RegisterDiagnosisCommand>("application/json")
			.Produces<RegisterDiagnosisResult>(StatusCodes.Status201Created)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.AddEndpointFilter<EndpointValidationFilter<RegisterDiagnosisCommand>>();
		}
	}
}
