using DiagnosesRegistry.Common;
using DiagnosesRegistry.Common.Interfaces;
using System.Windows.Input;


namespace DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Interfaces
{
	public interface IRegisterDiagnosisHandler : ICommandHandler<RegisterDiagnosisCommand, CommandResult<RegisterDiagnosisResult>>
	{
		Task<CommandResult<RegisterDiagnosisResult>> HandleAsync(RegisterDiagnosisCommand command, CancellationToken ct);
	}
}
