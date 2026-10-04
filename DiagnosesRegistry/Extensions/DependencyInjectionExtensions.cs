using DiagnosesRegistry.Common;
using DiagnosesRegistry.DiagnosisRegistry;
using DiagnosesRegistry.DiagnosisRegistry.HelperSevices;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Interfaces;
using DiagnosesRegistry.DiagnosisRegistry.MedicalEnpoints.RegisterEdpoint.Validators;
using DiagnosesRegistry.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DiagnosesRegistry.Extensions
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{

			services.AddDbContext<DiagnosesDbContext>(options =>
				options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
					?? "Data Source=diagnoses.db"));

		
			services.AddSingleton<IICD10ValueSetService, ICD10ValueSetService>();

		
			services.AddScoped<RegisterDiagnosisCommandValidator>();

		
			services.AddScoped<RegisterDiagnosisHandler>();
			services.AddScoped<IRegisterDiagnosisHandler, RegisterDiagnosisLoggingDecorator>();

			return services;
		}
	}
}
