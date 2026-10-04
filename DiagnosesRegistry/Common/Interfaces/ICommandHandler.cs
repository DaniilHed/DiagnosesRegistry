namespace DiagnosesRegistry.Common.Interfaces
{
	public interface ICommandHandler<in TCommand, TResult>
	{
		Task<TResult> HandleAsync(TCommand command, CancellationToken ct);
	}
}
