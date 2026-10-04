using DiagnosesRegistry.Common;
using DiagnosesRegistry.Common.Errors;
using DiagnosesRegistry.DiagnoseRegistry.DomainEntities;
using DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses;
using DiagnosesRegistry.DiagnosisRegistry.Specificartions;

namespace DiagnosesRegistry.DiagnosisRegistry.DomainEntities
{
	public class DiagnosisBuilder
	{
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

		public DiagnosisBuilder ForPatient(Guid patientId)
		{
			PatientId = patientId;
			return this;
		}

		public DiagnosisBuilder FromSource(string system, string recordId)
		{
			SourceSystem = system;
			SourceRecordId = recordId;
			return this;
		}

		public DiagnosisBuilder WithCode(string code, string system, string display)
		{
			Icd10Code = code;
			CodingSystem = system;
			DisplayText = display;
			return this;
		}

		public DiagnosisBuilder WithTiming(DateTime postDate, DateTime? onsetDate = null, int? onsetAgeYears = null)
		{
			PostDate = postDate;
			OnsetDate = onsetDate;
			OnsetAgeYears = onsetAgeYears;
			return this;
		}

		public DiagnosisBuilder WithStatuses(ClinicalStatus clinical, VerificationStatus verification)
		{
			ClinicalStatus = clinical;
			VerificationStatus = verification;
			return this;
		}

		public CommandResult<Diagnosis> Build()
		{
			if (PatientId == Guid.Empty)
			{
				throw new InvalidOperationException("Cannot build Diagnosis: PatientId was not set.");
			}

			if (string.IsNullOrWhiteSpace(SourceSystem) || string.IsNullOrWhiteSpace(SourceRecordId))
			{
				throw new InvalidOperationException("Cannot build Diagnosis: SourceSystem and SourceRecordId are required.");
			}

			if (!DiagnosisSpecifications.PostDateNotInFuture.IsSatisfiedBy(this))
			{
				return DiagnosisErrors.FuturePostDate;
			}

			if (!DiagnosisSpecifications.OnsetMutualExclusivity.IsSatisfiedBy(this))
			{
				return DiagnosisErrors.MutuallyExclusiveOnset;
			}

			if (!DiagnosisSpecifications.ValidCoding.IsSatisfiedBy(this))
			{
				return DiagnosisErrors.MissingCode;
			}

			var entity = Diagnosis.Create(
			PatientId,
			SourceSystem,
			SourceRecordId,
			Icd10Code,
			CodingSystem,
			DisplayText,
			PostDate,
			ClinicalStatus,
			VerificationStatus,
			OnsetDate,
			OnsetAgeYears
		);

			return CommandResult<Diagnosis>.Success(entity);
		}
	}
}
