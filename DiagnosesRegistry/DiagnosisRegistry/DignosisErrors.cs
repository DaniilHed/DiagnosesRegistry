using DiagnosesRegistry.Common.Errors;

namespace DiagnosesRegistry.DiagnosisRegistry
{
	public static class DiagnosisErrors
	{
		// Date and age validation errors
		public static readonly CustomError FuturePostDate =
			new("Diagnosis.FuturePostDate", "The post date of the diagnosis cannot be in the future.");

		public static readonly CustomError MissingOnset =
			new("Diagnosis.MissingOnset", "Providing the onset of symptoms (either date or age) is required.");

		public static readonly CustomError MutuallyExclusiveOnset =
			new("Diagnosis.MutuallyExclusiveOnset", "Cannot provide both onset date and onset age simultaneously.");

		// Coding system and ValueSet dictionary errors
		public static readonly CustomError MissingCode =
			new("Diagnosis.MissingCode", "The ICD-10 code is required.");

		public static readonly CustomError InvalidCodingSystem =
			new("Diagnosis.InvalidCodingSystem", "An invalid or unsupported coding system was provided.");

		public static readonly CustomError DeprecatedCode =
			new("Diagnosis.DeprecatedCode", "The selected ICD-10 code has been deprecated from the ValueSet.");

		// State transition errors 
		public static readonly CustomError InvalidStatusTransition =
			new("Diagnosis.InvalidStatusTransition", "Invalid status transition: cannot change status from 'Resolved' back to 'Active'.");

		// Dynamic error factory methods for detailed messages
		public static CustomError CodeNotFound(string code) =>
			new("Diagnosis.CodeNotFound", $"The ICD-10 code '{code}' was not found in the allowed ValueSet.");

		public static CustomError NotFound(Guid id) =>
			new("Diagnosis.NotFound", $"Diagnosis with identifier '{id}' was not found.");

		public static CustomError ExternalRegistryFailure(string details) =>
			new("Diagnosis.ExternalRegistryFailure", $"Submission to the external registry failed: {details}");
	}
}
