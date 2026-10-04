using DiagnosesRegistry.Common.Errors;
using DiagnosesRegistry.DiagnosisRegistry.Dtos;
using System.Text.Json;

namespace DiagnosesRegistry.DiagnosisRegistry.HelperSevices
{
	public interface IICD10ValueSetService
	{
		string CanonicalUrl { get; }
		string SystemCodingUrl { get; }
		bool TryValidateAndGetDisplay(string code, string codingSystem, out string? officialDisplay, out CustomError? error);
	}
	public class ICD10ValueSetService : IICD10ValueSetService
	{
		private readonly Dictionary<string, string> _codeToDisplayMap = new(StringComparer.OrdinalIgnoreCase);

		public string CanonicalUrl { get; }
		public string SystemCodingUrl { get; }

		public ICD10ValueSetService(IHostEnvironment env)
		{
			// Resolve path to the ICD-10 ValueSet JSON file
			var filePath = System.IO.Path.Combine(env.ContentRootPath, "icd10-valueset.json");

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"ValueSet resource file was not found at path: {filePath}");
			}

			// Read and deserialize the ValueSet JSON resource
			var jsonContent = File.ReadAllText(filePath);
			var valueSet = JsonSerializer.Deserialize<ValueSetResourceDto>(jsonContent)
						   ?? throw new InvalidOperationException("Failed to deserialize the ValueSet resource.");

			// Extract  URL metadata
			CanonicalUrl = valueSet.Url;

			// Get concept rules from the FHIR structure
			var include = valueSet.Compose?.Include?.FirstOrDefault()
						  ?? throw new InvalidOperationException("The ValueSet resource does not contain a valid compose.include section.");

			SystemCodingUrl = include.System;

			//  lookup dictionary with code to display mappings
			foreach (var concept in include.Concept)
			{
				_codeToDisplayMap[concept.Code] = concept.Display;
			}
		}

		public bool TryValidateAndGetDisplay(
			string code,
			string codingSystem,
			out string? officialDisplay,
			out CustomError? error)
		{
			officialDisplay = null;
			error = null;

			
			if (!string.Equals(codingSystem, SystemCodingUrl, StringComparison.OrdinalIgnoreCase) &&
				!string.Equals(codingSystem, "ICD-10", StringComparison.OrdinalIgnoreCase))
			{
				error = DiagnosisErrors.InvalidCodingSystem;
				return false;
			}

			
			if (!_codeToDisplayMap.TryGetValue(code, out officialDisplay))
			{
				error = DiagnosisErrors.CodeNotFound(code);
				return false;
			}

			
			if (string.Equals(code, "A00", StringComparison.OrdinalIgnoreCase))
			{
				officialDisplay = null;
				error = DiagnosisErrors.DeprecatedCode;
				return false;
			}

			return true;
		}
	}
}

	