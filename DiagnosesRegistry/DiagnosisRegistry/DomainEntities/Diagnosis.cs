using DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses;

namespace DiagnosesRegistry.DiagnoseRegistry.DomainEntities
{
	public class Diagnosis
	{
		public Guid Id { get; private set; }
		public Guid PatientId { get; private set; }
		public string SourceSystem { get; private set; } = default!;
		public string SourceRecordId { get; private set; } = default!;
		public string Icd10Code { get; private set; } = default!;
		public string CodingSystem { get; private set; } = default!;
		public string DisplayText { get; private set; } = default!;
		public DateTime PostDate { get; private set; }
		public ClinicalStatus ClinicalStatus { get; private set; }
		public VerificationStatus VerificationStatus { get; private set; }
		public DateTime? OnsetDate { get; private set; }
		public int? OnsetAgeYears { get; private set; }
		public ExternalRegistryStatus RegistryStatus { get; private set; }
		public string? LastRegistryError { get; private set; }
		private Diagnosis() { }

		public static Diagnosis Create(
			Guid patientId,
			string sourceSystem,
			string sourceRecordId,
			string icd10Code,
			string codingSystem,
			string displayText,
			DateTime postDate,
			ClinicalStatus clinicalStatus,
			VerificationStatus verificationStatus,
			DateTime? onsetDate = null,
			int? onsetAgeYears = null)
		{
			return new Diagnosis
			{
				Id = Guid.NewGuid(),
				PatientId = patientId,
				SourceSystem = sourceSystem,
				SourceRecordId = sourceRecordId,
				Icd10Code = icd10Code,
				CodingSystem = codingSystem,
				DisplayText = displayText,
				PostDate = postDate,
				ClinicalStatus = clinicalStatus,
				VerificationStatus = verificationStatus,
				OnsetDate = onsetDate,
				OnsetAgeYears = onsetAgeYears,
				RegistryStatus = ExternalRegistryStatus.Pending
			};
		}
		public void MarkAsAccepted()
		{
			
			if (RegistryStatus == ExternalRegistryStatus.Accepted)
			{
				return;
			}

			RegistryStatus = ExternalRegistryStatus.Accepted;
			LastRegistryError = null; 
		}

		public void MarkAsFailed(string? errorMessage)
		{
			
			if (RegistryStatus == ExternalRegistryStatus.Accepted)
			{
				throw new InvalidOperationException("Cannot mark an error for a diagnosis that has already been accepted in the registry.");
			}

			RegistryStatus = ExternalRegistryStatus.Failed;

			LastRegistryError = string.IsNullOrWhiteSpace(errorMessage)
				? "Unknown registration error."
				: errorMessage;
		}
	}
}
