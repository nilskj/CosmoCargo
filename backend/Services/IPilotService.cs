using CosmoCargo.Model;

namespace CosmoCargo.Services
{
    public interface IPilotService
    {
        Task<IEnumerable<User>> GetAllPilotsAsync();
        Task<User?> GetPilotByIdAsync(Guid id);
        Task<bool> IsPilotAvailableAsync(Guid pilotId);
        Task<int> GetPilotShipmentCountAsync(Guid pilotId);
    }
} 