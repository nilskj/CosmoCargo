using CosmoCargo.Models;

namespace CosmoCargo.Services
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        public RiskLevel AssessRisk(TollForm tollForm)
        {
            if (tollForm.IsPlasmaActive && 
                tollForm.PlasmaStabilityLevel.HasValue && 
                tollForm.PlasmaStabilityLevel.Value < 5 && 
                tollForm.ContainsLifeforms)
            {
                return RiskLevel.Critical;
            }

            if (tollForm.ContainsLifeforms && 
                (string.IsNullOrWhiteSpace(tollForm.LifeformType) || 
                 tollForm.LifeformType.ToLower().Contains("okänd") || 
                 tollForm.LifeformType.ToLower().Contains("unknown")))
            {
                return RiskLevel.High;
            }

            if (tollForm.IsPlasmaActive && 
                tollForm.PlasmaStabilityLevel.HasValue && 
                tollForm.PlasmaStabilityLevel.Value >= 5 && 
                tollForm.PlasmaStabilityLevel.Value <= 7)
            {
                return RiskLevel.Medium;
            }

            return RiskLevel.Low;
        }
    }
} 