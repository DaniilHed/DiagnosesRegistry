using DiagnosesRegistry.Common.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace DiagnosesRegistry.Common
{
	public class LoggingCommandHandlerDecorator<TCommand, TResult>(
	ICommandHandler<TCommand, TResult> innerHandler,
	ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> logger)
	: ICommandHandler<TCommand, TResult>
	{
		public async Task<TResult> HandleAsync(TCommand command, CancellationToken ct)
		{
			var commandName = typeof(TCommand).Name;
			var timer = Stopwatch.StartNew();

			logger.LogInformation("Processing {CommandName} | Payload: {Payload}",
				commandName, JsonSerializer.Serialize(command));

			try
			{
				var result = await innerHandler.HandleAsync(command, ct);
				timer.Stop();

				logger.LogInformation("Completed {CommandName} in {Elapsed} ms",
					commandName, timer.ElapsedMilliseconds);

				return result;
			}
			catch (Exception ex)
			{
				timer.Stop();
				logger.LogError(ex, "Command {CommandName} failed after {Elapsed} ms",
					commandName, timer.ElapsedMilliseconds);
				throw;
			}
		}
	}
}
