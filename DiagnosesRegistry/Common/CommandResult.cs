using DiagnosesRegistry.Common.Errors;

namespace DiagnosesRegistry.Common
{
	public class CommandResult
	{
		public bool IsSuccess { get; }
		public bool IsFailure => !IsSuccess;

		public CustomError Error;

		private CommandResult() { }
		protected CommandResult(bool isSuccess, CustomError error)
		{
			if (!isSuccess && error == CustomError.None)
			{
				throw new ArgumentException("Error field cannont be empty for failure result.");
			}
			if (isSuccess && error != CustomError.None)
			{
				throw new ArgumentException("Error field must be empty for successfull result.");
			}
			IsSuccess = isSuccess;
			Error = error;
		}

		public static CommandResult Success() => new(true, CustomError.None);
		public static CommandResult Failure(CustomError error) => new(false, error);

		public static CommandResult<TValue> Success<TValue>(TValue value) => new(value, true, CustomError.None);
		public static CommandResult<TValue> Failure<TValue>(CustomError error) => new(default, false, error);

	}

	public class CommandResult<TValue> : CommandResult
	{
		private TValue? _value;
		public TValue Value => IsSuccess ?
			_value! :
			throw new ArgumentException("Value cannot be accessed");
		protected internal CommandResult(TValue? value, bool isSuccess, CustomError error) : base(isSuccess, error)
		{
			if (isSuccess && value == null)
			{
				throw new ArgumentException("Value field cannot be null for successfull result.");
			}
			if (!isSuccess && value != null)
			{
				throw new ArgumentException("Value field must be null for failure result.");
			}
			_value = value;
		}
		public static implicit operator CommandResult<TValue>(TValue? value) =>
			value is not null
			? Success(value)
			: Failure<TValue>(CustomError.NullValue);
		public static implicit operator CommandResult<TValue>(CustomError error) => Failure<TValue>(error);

	}
}
