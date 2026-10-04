namespace DiagnosesRegistry.DiagnoseRegistry.DomainEntities
{
	public class Patient
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string? Pesel { get; set; }
		public string ChartNumber { get; set; } = default!;
		public string SourceSystem { get; set; } = default!; 
		public List<Diagnosis> Diagnoses { get; set; } = new();
	}
}
