using CosmoCargo.Data;
using CosmoCargo.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmoCargo.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _context;
        private readonly IRiskAssessmentService _riskAssessmentService;

        public ShipmentService(AppDbContext context, IRiskAssessmentService riskAssessmentService)
        {
            _context = context;
            _riskAssessmentService = riskAssessmentService;
        }

        public async Task<IEnumerable<Shipment>> GetAllShipmentsAsync()
        {
            return await _context.Shipments
                .Include(s => s.Customer)
                .Include(s => s.Pilot)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByCustomerIdAsync(Guid customerId)
        {
            return await _context.Shipments
                .Include(s => s.Pilot)
                .Where(s => s.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByPilotIdAsync(Guid pilotId)
        {
            return await _context.Shipments
                .Include(s => s.Customer)
                .Where(s => s.PilotId == pilotId)
                .ToListAsync();
        }

        public async Task<Shipment?> GetShipmentByIdAsync(Guid id)
        {
            return await _context.Shipments
                .Include(s => s.Customer)
                .Include(s => s.Pilot)
                .Include(s => s.TollForm)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Shipment> CreateShipmentAsync(Shipment shipment)
        {
            shipment.Id = Guid.NewGuid();
            shipment.CreatedAt = DateTime.UtcNow;
            shipment.UpdatedAt = DateTime.UtcNow;
            shipment.Status = ShipmentStatus.Pending;
            shipment.RiskLevel = RiskLevel.Low;

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return shipment;
        }

        public async Task<Shipment?> UpdateShipmentStatusAsync(Guid id, ShipmentStatus status)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
                return null;

            shipment.Status = status;
            shipment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return shipment;
        }

        public async Task<Shipment?> AssignPilotAsync(Guid shipmentId, Guid pilotId)
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);
            if (shipment == null)
                return null;

            var pilot = await _context.Users.FirstOrDefaultAsync(u => u.Id == pilotId && u.Role == UserRole.Pilot);
            if (pilot == null)
                return null;

            shipment.PilotId = pilotId;
            shipment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return shipment;
        }
    }
} 