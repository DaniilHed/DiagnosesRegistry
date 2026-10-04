using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum VerificationStatus
	{
		[JsonPropertyName("unconfirmed")]
		Unconfirmed,

		[JsonPropertyName("provisional")]
		Provisional,

		[JsonPropertyName("differential")]
		Differential,

		[JsonPropertyName("confirmed")]
		Confirmed,

		[JsonPropertyName("refuted")]
		Refuted,

		[JsonPropertyName("entered-in-error")]
		EnteredInError
	}
}
