using DiagnosesRegistry.Common;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Interfaces;

namespace DiagnosesRegistry.DiagnosisRegistry
{
	public class RegisterDiagnosisLoggingDecorator(
	RegisterDiagnosisHandler innerHandler,
	ILogger<LoggingCommandHandlerDecorator<RegisterDiagnosisCommand, CommandResult<RegisterDiagnosisResult>>> logger)
	: LoggingCommandHandlerDecorator<RegisterDiagnosisCommand, CommandResult<RegisterDiagnosisResult>>(innerHandler, logger),
	  IRegisterDiagnosisHandler;
}
