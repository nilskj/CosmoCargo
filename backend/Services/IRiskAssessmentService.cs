using CosmoCargo.Models;

namespace CosmoCargo.Services
{
    public interface IRiskAssessmentService
    {
        RiskLevel AssessRisk(TollForm tollForm);
    }
} 