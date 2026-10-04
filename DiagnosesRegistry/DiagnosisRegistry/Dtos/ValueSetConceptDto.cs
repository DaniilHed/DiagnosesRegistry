using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.Dtos
{
	public record ValueSetConceptDto(
	[property: JsonPropertyName("code")] string Code,
	[property: JsonPropertyName("display")] string Display);
}
