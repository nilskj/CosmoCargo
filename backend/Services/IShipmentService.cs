using CosmoCargo.Model;
using CosmoCargo.Model.Queries;
namespace CosmoCargo.Services
{
    public interface IShipmentService
    {
        Task<IEnumerable<Shipment>> GetShipmentsAsync(ShipmentsFilter filter);
        Task<IEnumerable<Shipment>> GetShipmentsByCustomerIdAsync(Guid customerId, ShipmentsFilter filter);
        Task<IEnumerable<Shipment>> GetShipmentsByPilotIdAsync(Guid pilotId, ShipmentsFilter filter);
        Task<Shipment?> GetShipmentByIdAsync(Guid id);
        Task<Shipment> CreateShipmentAsync(Shipment shipment);
        Task<Shipment?> UpdateShipmentStatusAsync(Guid id, ShipmentStatus status);
        Task<Shipment?> AssignPilotAsync(Guid shipmentId, Guid pilotId);
    }
} 