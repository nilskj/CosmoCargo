using CosmoCargo.Data;
using CosmoCargo.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmoCargo.Services
{
    public class TollFormService : ITollFormService
    {
        private readonly AppDbContext _context;
        private readonly IRiskAssessmentService _riskAssessmentService;

        public TollFormService(AppDbContext context, IRiskAssessmentService riskAssessmentService)
        {
            _context = context;
            _riskAssessmentService = riskAssessmentService;
        }

        public async Task<IEnumerable<TollForm>> GetAllTollFormsAsync()
        {
            return await _context.TollForms
                .Include(t => t.Shipment)
                .Include(t => t.SubmittedBy)
                .ToListAsync();
        }

        public async Task<TollForm?> GetTollFormByIdAsync(Guid id)
        {
            return await _context.TollForms
                .Include(t => t.Shipment)
                .Include(t => t.SubmittedBy)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TollForm?> GetTollFormByShipmentIdAsync(Guid shipmentId)
        {
            return await _context.TollForms
                .Include(t => t.Shipment)
                .Include(t => t.SubmittedBy)
                .FirstOrDefaultAsync(t => t.ShipmentId == shipmentId);
        }

        public async Task<TollForm> CreateTollFormAsync(TollForm tollForm)
        {
            // Validera tullformuläret
            ValidateTollForm(tollForm);

            tollForm.Id = Guid.NewGuid();
            tollForm.CreatedAt = DateTime.UtcNow;
            tollForm.IsApproved = null; // Inte granskad än

            _context.TollForms.Add(tollForm);

            // Uppdatera risknivån för frakten
            var shipment = await _context.Shipments.FindAsync(tollForm.ShipmentId);
            if (shipment != null)
            {
                shipment.RiskLevel = _riskAssessmentService.AssessRisk(tollForm);
                shipment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return tollForm;
        }

        public async Task<TollForm?> ReviewTollFormAsync(Guid id, bool isApproved, string? reviewNotes, Guid reviewedById)
        {
            var tollForm = await _context.TollForms.FindAsync(id);
            if (tollForm == null)
                return null;

            tollForm.IsApproved = isApproved;
            tollForm.ReviewNotes = reviewNotes;
            tollForm.ReviewedById = reviewedById;
            tollForm.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return tollForm;
        }

        private void ValidateTollForm(TollForm tollForm)
        {
            // Validera enligt reglerna
            if (tollForm.ContainsLifeforms && string.IsNullOrWhiteSpace(tollForm.LifeformType))
            {
                throw new ArgumentException("Lifeform type is required when contains lifeforms is true");
            }

            if (tollForm.IsPlasmaActive && !tollForm.PlasmaStabilityLevel.HasValue)
            {
                throw new ArgumentException("Plasma stability level is required when is plasma active is true");
            }

            if (tollForm.IsPlasmaActive && tollForm.PlasmaStabilityLevel < 4 && !tollForm.QuarantineRequired)
            {
                throw new ArgumentException("Quarantine is required for plasma stability level below 4");
            }

            if (!tollForm.OriginPlanetLawsConfirmed)
            {
                throw new ArgumentException("Origin planet laws must be confirmed");
            }
        }
    }
} 