using DiagnosesRegistry.DiagnoseRegistry.DomainEntities;
using Microsoft.EntityFrameworkCore;

namespace DiagnosesRegistry.Infrastructure
{
	public class DiagnosesDbContext : DbContext
	{
		public DiagnosesDbContext(DbContextOptions<DiagnosesDbContext> options) : base(options)
		{
		}

		public DbSet<Patient> Patients => Set<Patient>();
		public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// 1. ChartNumber + SourceSystem must be unique together (SYS-A and SYS-B are different people)
			modelBuilder.Entity<Patient>()
				.HasIndex(p => new { p.ChartNumber, p.SourceSystem })
				.IsUnique();

			// 2. Idempotency check: Record ID per Source System must be unique
			modelBuilder.Entity<Diagnosis>()
				.HasIndex(d => new { d.SourceSystem, d.SourceRecordId })
				.IsUnique();

			// 3. Store Enums as readable Strings in SQLite instead of numbers
			modelBuilder.Entity<Diagnosis>(b =>
			{
				b.Property(d => d.ClinicalStatus).HasConversion<string>();
				b.Property(d => d.VerificationStatus).HasConversion<string>();
				b.Property(d => d.RegistryStatus).HasConversion<string>();
			});
		}
	}
}