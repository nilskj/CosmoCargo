using CosmoCargo.Models;
using CosmoCargo.Services;
using CosmoCargo.Utils;
using System.Security.Claims;

namespace CosmoCargo.Endpoints
{
    public static class PilotEndpoints
    {
        public static async Task<IResult> GetAllPilots(
            IPilotService pilotService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            if (role != UserRole.Admin.ToString())
                return Results.Forbid();

            var pilots = await pilotService.GetAllPilotsAsync();
            return Results.Ok(pilots);
        }

        public static async Task<IResult> GetPilotById(
            Guid id,
            IPilotService pilotService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            if (role != UserRole.Admin.ToString())
                return Results.Forbid();

            var pilot = await pilotService.GetPilotByIdAsync(id);
            if (pilot == null)
                return Results.NotFound();

            return Results.Ok(pilot);
        }

        public static async Task<IResult> GetPilotAvailability(
            Guid id,
            IPilotService pilotService,
            ClaimsPrincipal user)
        {
            var role = user.GetRole();
            if (role != UserRole.Admin.ToString())
                return Results.Forbid();

            var pilot = await pilotService.GetPilotByIdAsync(id);
            if (pilot == null)
                return Results.NotFound();

            var isAvailable = await pilotService.IsPilotAvailableAsync(id);
            var activeShipments = await pilotService.GetPilotShipmentCountAsync(id);

            return Results.Ok(new
            {
                IsAvailable = isAvailable,
                ActiveShipments = activeShipments,
                MaxShipments = 3
            });
        }
    }
} 