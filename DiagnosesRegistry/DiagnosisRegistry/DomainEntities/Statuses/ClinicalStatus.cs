using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ClinicalStatus
	{
		Active,
		Recurrence,
		Relapse,
		Inactive,
		Remission,
		Resolved
	}
}
