using DiagnosesRegistry.DiagnoseRegistry.DomainEntities;
using DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses;

namespace DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint
{
	public record RegisterDiagnosisResult(Guid Id,
	string SourceSystem,
	string SourceRecordId,
	string Icd10Code,
	string DisplayText,
	ExternalRegistryStatus RegistryStatus,
	string? LastRegistryError = null)
	{
		public static RegisterDiagnosisResult FromEntity(Diagnosis d) => new(
			d.Id,
			d.SourceSystem,
			d.SourceRecordId,
			d.Icd10Code,
			d.DisplayText,
			d.RegistryStatus
		);
	}
}
