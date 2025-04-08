using CosmoCargo.Models;
using CosmoCargo.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CosmoCargo.Endpoints
{
    public static class ShipmentEndpoints
    {
        public static async Task<IResult> GetAllShipments(
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            // Hämta användarens roll
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            IEnumerable<Shipment> shipments;

            // Filtrera frakter baserat på användarens roll
            if (role == UserRole.Admin.ToString())
            {
                // Admin ser alla frakter
                shipments = await shipmentService.GetAllShipmentsAsync();
            }
            else if (role == UserRole.Pilot.ToString())
            {
                // Pilot ser sina tilldelade frakter
                shipments = await shipmentService.GetShipmentsByPilotIdAsync(userId);
            }
            else if (role == UserRole.Customer.ToString())
            {
                // Kund ser sina egna frakter
                shipments = await shipmentService.GetShipmentsByCustomerIdAsync(userId);
            }
            else
            {
                return Results.Forbid();
            }

            return Results.Ok(shipments);
        }

        public static async Task<IResult> GetShipmentById(
            Guid id,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var shipment = await shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
            {
                return Results.NotFound();
            }

            // Kontrollera behörighet
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            if (role == UserRole.Admin.ToString())
            {
                // Admin kan se alla frakter
                return Results.Ok(shipment);
            }
            else if (role == UserRole.Pilot.ToString() && shipment.PilotId == userId)
            {
                // Pilot kan se sina tilldelade frakter
                return Results.Ok(shipment);
            }
            else if (role == UserRole.Customer.ToString() && shipment.CustomerId == userId)
            {
                // Kund kan se sina egna frakter
                return Results.Ok(shipment);
            }

            return Results.Forbid();
        }

        public static async Task<IResult> CreateShipment(
            CreateShipmentRequest request,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var shipment = new Shipment
            {
                CustomerId = userId,
                Origin = request.Origin,
                Destination = request.Destination,
                Weight = request.Weight,
                Category = request.Category,
                Priority = request.Priority
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
            // Kontrollera behörighet
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var shipment = await shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
            {
                return Results.NotFound();
            }

            // Piloter kan bara uppdatera sina egna frakter
            if (role == UserRole.Pilot.ToString() && shipment.PilotId != userId)
            {
                return Results.Forbid();
            }

            var updatedShipment = await shipmentService.UpdateShipmentStatusAsync(id, request.Status);
            if (updatedShipment == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(updatedShipment);
        }

        public static async Task<IResult> AssignPilot(
            Guid id,
            AssignPilotRequest request,
            IShipmentService shipmentService)
        {
            var updatedShipment = await shipmentService.AssignPilotAsync(id, request.PilotId);
            if (updatedShipment == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(updatedShipment);
        }
    }

    public class CreateShipmentRequest
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
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
