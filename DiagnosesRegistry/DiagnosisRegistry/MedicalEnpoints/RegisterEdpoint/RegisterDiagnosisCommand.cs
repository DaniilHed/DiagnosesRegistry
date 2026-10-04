using DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses;

namespace DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint
{
	public record RegisterDiagnosisCommand(
		string SourceSystem,
		string SourceRecordId,
		string? Pesel,
		string ChartNumber,
		string Code,
		string CodingSystem,
		string InputDescription, 
		DateTime PostDate,
		ClinicalStatus ClinicalStatus,
		VerificationStatus VerificationStatus,
		DateTime? OnsetDate,
		int? OnsetAgeYears,
		string? EncounterId
		);
	
}
