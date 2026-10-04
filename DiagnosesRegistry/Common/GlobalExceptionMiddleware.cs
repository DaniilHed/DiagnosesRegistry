using Microsoft.AspNetCore.Mvc;

namespace DiagnosesRegistry.Common
{
	public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Server Error",
				Detail = "An internal server error occurred. Please try again later.",
				Instance = context.Request.Path
			};

			return context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}
