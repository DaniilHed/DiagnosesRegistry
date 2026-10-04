using DiagnosesRegistry.DiagnosisRegistry.HelperSevices;
using FluentValidation;

namespace DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Validators
{
	public class RegisterDiagnosisCommandValidator : AbstractValidator<RegisterDiagnosisCommand>
	{
		public RegisterDiagnosisCommandValidator(IICD10ValueSetService valueSetService)
		{
			RuleFor(x => x.SourceSystem)
				.NotEmpty().WithMessage("Source system is required.")
				.MaximumLength(20).WithMessage("Source system name cannot exceed 20 characters.");

			RuleFor(x => x.SourceRecordId)
				.NotEmpty().WithMessage("Source record identifier is required.")
				.MaximumLength(100).WithMessage("Source record identifier cannot exceed 100 characters.");

			RuleFor(x => x.ChartNumber)
				.NotEmpty().WithMessage("Patient chart number is required.")
				.MaximumLength(50).WithMessage("Chart number cannot exceed 50 characters.");

			RuleFor(x => x.Pesel)
				.Matches(@"^\d{11}$").WithMessage("PESEL number must consist of exactly 11 digits.")
				.When(x => !string.IsNullOrEmpty(x.Pesel));

			RuleFor(x => x.PostDate)
				.LessThanOrEqualTo(_ => DateTime.UtcNow)
				.WithMessage("Diagnosis date cannot be in the future.");

			RuleFor(x => x)
				.Must(x => (x.OnsetDate.HasValue && !x.OnsetAgeYears.HasValue) || (!x.OnsetDate.HasValue && x.OnsetAgeYears.HasValue))
				.WithMessage("Symptom onset is required and must be provided EITHER as a date (OnsetDate) OR as patient age (OnsetAge) – never both.")
				.WithName("Onset");

			RuleFor(x => x.OnsetAgeYears)
				.InclusiveBetween(0, 150).WithMessage("Patient age at symptom onset must be between 0 and 150 years.")
				.When(x => x.OnsetAgeYears.HasValue);

			RuleFor(x => x)
				.Custom((command, context) =>
				{
					if (!valueSetService.TryValidateAndGetDisplay(command.Code, command.CodingSystem, out _, out var error))
					{
						context.AddFailure(error!.Code, error.Description);
					}
				});
		}
	}
}
