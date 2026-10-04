using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.Dtos
{
	public record ValueSetResourceDto(
	[property: JsonPropertyName("resourceType")] string ResourceType,
	[property: JsonPropertyName("id")] string Id,
	[property: JsonPropertyName("url")] string Url,
	[property: JsonPropertyName("version")] string Version,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("status")] string Status,
	[property: JsonPropertyName("compose")] ValueSetComposeDto Compose);
}
