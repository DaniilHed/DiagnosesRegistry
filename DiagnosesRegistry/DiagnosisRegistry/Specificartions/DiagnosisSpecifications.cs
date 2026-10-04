using DiagnosesRegistry.Common;
using DiagnosesRegistry.DiagnosisRegistry.DomainEntities;

namespace DiagnosesRegistry.DiagnosisRegistry.Specificartions
{
	public static class DiagnosisSpecifications
	{
	
		public static readonly Specification<DiagnosisBuilder> PostDateNotInFuture =
			new(b => b.PostDate <= DateTime.UtcNow);
		public static readonly Specification<DiagnosisBuilder> OnsetMutualExclusivity =
			new(b => !(b.OnsetDate.HasValue && b.OnsetAgeYears.HasValue));
		public static readonly Specification<DiagnosisBuilder> ValidCoding =
			new(b => !string.IsNullOrWhiteSpace(b.Icd10Code) &&
					   !string.IsNullOrWhiteSpace(b.CodingSystem));
	}
}
