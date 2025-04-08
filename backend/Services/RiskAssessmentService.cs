using CosmoCargo.Models;

namespace CosmoCargo.Services
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        public RiskLevel AssessRisk(TollForm tollForm)
        {
            // Kritisk risk: Plasmaaktiv + stabilitet < 5 + livsform
            if (tollForm.IsPlasmaActive && 
                tollForm.PlasmaStabilityLevel.HasValue && 
                tollForm.PlasmaStabilityLevel.Value < 5 && 
                tollForm.ContainsLifeforms)
            {
                return RiskLevel.Critical;
            }

            // Hög risk: Livsform + okänd art
            if (tollForm.ContainsLifeforms && 
                (string.IsNullOrWhiteSpace(tollForm.LifeformType) || 
                 tollForm.LifeformType.ToLower().Contains("okänd") || 
                 tollForm.LifeformType.ToLower().Contains("unknown")))
            {
                return RiskLevel.High;
            }

            // Medel risk: Plasmaaktiv med stabilitet 5-7
            if (tollForm.IsPlasmaActive && 
                tollForm.PlasmaStabilityLevel.HasValue && 
                tollForm.PlasmaStabilityLevel.Value >= 5 && 
                tollForm.PlasmaStabilityLevel.Value <= 7)
            {
                return RiskLevel.Medium;
            }

            // Låg risk: Inga specialegenskaper
            return RiskLevel.Low;
        }
    }
} 