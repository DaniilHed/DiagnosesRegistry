namespace DiagnosesRegistry.Common.Errors
{
	public record CustomError(string Code, string? Description = null)
	{
		public static readonly CustomError None = new(string.Empty);
		public static readonly CustomError NullValue = new("Error.NullValue", "The specified result value is null.");
		public static implicit operator CommandResult(CustomError error) => CommandResult.Failure(error);
	}
}
