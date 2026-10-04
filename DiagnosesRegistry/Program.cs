using DiagnosesRegistry.Common;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint;
using DiagnosesRegistry.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Diagnoses Registry API v1");
	});
}

app.UseHttpsRedirection();
app.MapRegisterDiagnosisEndpoint();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.Run();
