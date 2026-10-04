using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.Dtos
{
	public record ValueSetIncludeDto(
	[property: JsonPropertyName("system")] string System,
	[property: JsonPropertyName("concept")] List<ValueSetConceptDto> Concept);
}
