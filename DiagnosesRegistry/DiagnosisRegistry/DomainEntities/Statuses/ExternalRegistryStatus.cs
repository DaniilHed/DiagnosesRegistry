using System.Text.Json.Serialization;

namespace DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ExternalRegistryStatus
	{
		Pending,
		Accepted,
		Failed
	}
}
