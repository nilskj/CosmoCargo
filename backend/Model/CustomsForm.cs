
namespace CosmoCargo.Model
{
    public class CustomsForm
    {
        public bool ContainsLifeforms { get; set; }
        public string? LifeformType { get; set; } // Required if ContainsLifeforms
        public bool IsPlasmaActive { get; set; }
        public int? PlasmaStabilityLevel { get; set; } // 1-10, required if IsPlasmaActive
        public bool OriginPlanetLawsConfirmed { get; set; } // Must be checked
        public string? CustomsNotes { get; set; }
        public bool? QuarantineRequired { get; set; } // Required if PlasmaStabilityLevel < 4
    }
} 