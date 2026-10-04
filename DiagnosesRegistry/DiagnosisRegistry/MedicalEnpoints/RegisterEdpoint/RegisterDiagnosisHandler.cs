using DiagnosesRegistry.Common;
using DiagnosesRegistry.DiagnoseRegistry.DomainEntities;
using DiagnosesRegistry.DiagnosisRegistry.DomainEntities;
using DiagnosesRegistry.DiagnosisRegistry.DomainEntities.Statuses;
using DiagnosesRegistry.DiagnosisRegistry.HelperSevices;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Interfaces;
using DiagnosesRegistry.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint
{
	public class RegisterDiagnosisHandler(
	DiagnosesDbContext dbContext,
	IICD10ValueSetService valueSetService,
	IExternalRegistryClient registryClient,
	ILogger<RegisterDiagnosisHandler> logger) : IRegisterDiagnosisHandler
	{
		public async Task<CommandResult<RegisterDiagnosisResult>> HandleAsync(RegisterDiagnosisCommand command, CancellationToken ct)
		{
			//Validate the ICD-10 medical code with dictionary
			if (!valueSetService.TryValidateAndGetDisplay(command.Code, command.CodingSystem, out var officialDisplay, out var valueSetError))
			{
				return CommandResult.Failure<RegisterDiagnosisResult>(valueSetError!);
			}

			//Idempotency check  
			var existing = await dbContext.Diagnoses
				.FirstOrDefaultAsync(d => d.SourceSystem == command.SourceSystem && d.SourceRecordId == command.SourceRecordId, ct);

			if (existing != null)
			{
				// prevent duplicate record
				return CommandResult<RegisterDiagnosisResult>.Success(RegisterDiagnosisResult.FromEntity(existing));
			}

			//  Retrieve existing patient by PESEL or Chart Number 
			var patient = await dbContext.Patients.FirstOrDefaultAsync(p =>
				(!string.IsNullOrEmpty(command.Pesel) && p.Pesel == command.Pesel) ||
				(p.ChartNumber == command.ChartNumber && p.SourceSystem == command.SourceSystem), ct);
			//add builder pattern latter
			if (patient == null)
			{
				patient = new Patient
				{
					Id = Guid.NewGuid(),
					Pesel = command.Pesel,
					ChartNumber = command.ChartNumber,
					SourceSystem = command.SourceSystem
				};
				dbContext.Patients.Add(patient);
			}

			// build the Diagnosis using Builder 
			var builderResult = new DiagnosisBuilder()
				.ForPatient(patient.Id)
				.FromSource(command.SourceSystem, command.SourceRecordId)
				.WithCode(command.Code, command.CodingSystem, officialDisplay!)
				.WithTiming(command.PostDate, command.OnsetDate, command.OnsetAgeYears)
				.WithStatuses(command.ClinicalStatus, command.VerificationStatus)
				.Build();

			if (builderResult.IsFailure)
			{
				return CommandResult.Failure<RegisterDiagnosisResult>(builderResult.Error);
			}

			var diagnosis = builderResult.Value;

			// Database transaction (ACID )
			using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

			try
			{
				dbContext.Diagnoses.Add(diagnosis);
				await dbContext.SaveChangesAsync(ct);

				// External HTTP integration with the medical registry client
				var registryResult = await registryClient.SendAsync(diagnosis.Id, diagnosis.Icd10Code, ct);

				if (registryResult.IsSuccess)
				{
					diagnosis.MarkAsAccepted();
				}
				else
				{
					diagnosis.MarkAsFailed(registryResult.ErrorMessage);
					logger.LogWarning("Registering diagnosis {DiagnosisId} with external registry failed: {Error}",
						diagnosis.Id, registryResult.ErrorMessage);
				}
				//Persist updated status and commit transaction atomically
				await dbContext.SaveChangesAsync(ct);
				await transaction.CommitAsync(ct);
			}
			catch(Exception ex)
			{
				await transaction.RollbackAsync(ct);

				logger.LogError(ex, "An error occurred while processing diagnosis registration for ID {DiagnosisId}", diagnosis.Id);

				return CommandResult.Failure<RegisterDiagnosisResult>(
					DiagnosisErrors.ExternalRegistryFailure(ex.Message));
			}

			return CommandResult.Success(RegisterDiagnosisResult.FromEntity(diagnosis));
		}
	}
}
