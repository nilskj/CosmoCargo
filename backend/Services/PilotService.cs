using CosmoCargo.Data;
using CosmoCargo.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmoCargo.Services
{
    public class PilotService : IPilotService
    {
        private readonly AppDbContext _context;

        public PilotService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllPilotsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Pilot)
                .ToListAsync();
        }

        public async Task<User?> GetPilotByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Pilot);
        }

        public async Task<bool> IsPilotAvailableAsync(Guid pilotId)
        {
            var activeShipments = await _context.Shipments
                .CountAsync(s => s.PilotId == pilotId && 
                               s.Status != ShipmentStatus.Delivered && 
                               s.Status != ShipmentStatus.LostInBlackHole);

            return activeShipments < 3;
        }

        public async Task<int> GetPilotShipmentCountAsync(Guid pilotId)
        {
            return await _context.Shipments
                .CountAsync(s => s.PilotId == pilotId && 
                               s.Status != ShipmentStatus.Delivered && 
                               s.Status != ShipmentStatus.LostInBlackHole);
        }
    }
} 