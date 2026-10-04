using FluentValidation;

namespace DiagnosesRegistry.Common
{
	public class EndpointValidationFilter<T> : IEndpointFilter where T : class
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
			if (validator is null)
			{
				return await next(context);
			}

			var argument = context.Arguments.OfType<T>().FirstOrDefault();
			if (argument is null)
			{
				return Results.Problem(
					detail: "Request body cannot be empty.",
					statusCode: StatusCodes.Status400BadRequest
				);
			}

			var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors
					.GroupBy(e => e.PropertyName)
					.ToDictionary(
						g => g.Key,
						g => g.Select(e => e.ErrorMessage).ToArray()
					);

				return Results.ValidationProblem(
					errors: errors,
					detail: "Validation errors occurred.",
					instance: context.HttpContext.Request.Path,
					statusCode: StatusCodes.Status400BadRequest
				);
			}

			return await next(context);
		}
	}
}
