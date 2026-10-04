using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.Dtos
{
	public record ValueSetComposeDto([property: JsonPropertyName("include")] List<ValueSetIncludeDto> Include);
}
