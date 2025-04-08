using CosmoCargo.Models;

namespace CosmoCargo.Services
{
    public interface IShipmentService
    {
        Task<IEnumerable<Shipment>> GetAllShipmentsAsync();
        Task<IEnumerable<Shipment>> GetShipmentsByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Shipment>> GetShipmentsByPilotIdAsync(Guid pilotId);
        Task<Shipment?> GetShipmentByIdAsync(Guid id);
        Task<Shipment> CreateShipmentAsync(Shipment shipment);
        Task<Shipment?> UpdateShipmentStatusAsync(Guid id, ShipmentStatus status);
        Task<Shipment?> AssignPilotAsync(Guid shipmentId, Guid pilotId);
    }
} 