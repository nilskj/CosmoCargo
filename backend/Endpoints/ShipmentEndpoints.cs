using CosmoCargo.Model;
using CosmoCargo.Services;
using CosmoCargo.Utils;
using System.Security.Claims;

namespace CosmoCargo.Endpoints
{
    public static class ShipmentEndpoints
    {
        public static async Task<IResult> GetAllShipments(
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            var userId = user.GetUserId();

            IEnumerable<Shipment> shipments;

            if (role == UserRole.Admin.ToString())
                shipments = await shipmentService.GetAllShipmentsAsync();
            else if (role == UserRole.Pilot.ToString())
                shipments = await shipmentService.GetShipmentsByPilotIdAsync(userId);
            else if (role == UserRole.Customer.ToString())
                shipments = await shipmentService.GetShipmentsByCustomerIdAsync(userId);
            else
                return Results.Forbid();

            return Results.Ok(shipments);
        }

        public static async Task<IResult> GetShipmentById(
            Guid id,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var shipment = await shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return Results.NotFound();

            var role = user.GetRole();
            var userId = user.GetUserId();

            if (role == UserRole.Admin.ToString())
                return Results.Ok(shipment);
            else if (role == UserRole.Pilot.ToString() && shipment.PilotId == userId)
                return Results.Ok(shipment);
            else if (role == UserRole.Customer.ToString() && shipment.CustomerId == userId)
                return Results.Ok(shipment);

            return Results.Forbid();
        }

        public static async Task<IResult> CreateShipment(
            CreateShipmentRequest request,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            if (role == UserRole.Customer.ToString())
                return Results.Forbid();

            var userId = user.GetUserId();

            var shipment = new Shipment
            {
                CustomerId = userId,
                Origin = request.Origin,
                Destination = request.Destination,
                Weight = request.Weight,
                Cargo = request.Cargo,
                Priority = request.Priority,
                ScheduledDate = request.ScheduledDate,
                
            };

            var createdShipment = await shipmentService.CreateShipmentAsync(shipment);
            return Results.Created($"/api/shipments/{createdShipment.Id}", createdShipment);
        }

        public static async Task<IResult> UpdateShipmentStatus(
            Guid id,
            UpdateShipmentStatusRequest request,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            var userId = user.GetUserId();

            var shipment = await shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return Results.NotFound();

            if (role == UserRole.Pilot.ToString() && shipment.PilotId != userId)
                return Results.Forbid();

            var updatedShipment = await shipmentService.UpdateShipmentStatusAsync(id, request.Status);
            if (updatedShipment == null)
                return Results.NotFound();

            return Results.Ok(updatedShipment);
        }

        public static async Task<IResult> AssignPilot(
            Guid id,
            AssignPilotRequest request,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            if (role != UserRole.Admin.ToString())
                return Results.Forbid();

            var updatedShipment = await shipmentService.AssignPilotAsync(id, request.PilotId);
            if (updatedShipment == null)
                return Results.NotFound();

            return Results.Ok(updatedShipment);
        }
    }

    public class CreateShipmentRequest
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
    }

    public class UpdateShipmentStatusRequest
    {
        public ShipmentStatus Status { get; set; }
    }

    public class AssignPilotRequest
    {
        public Guid PilotId { get; set; }
    }
}
