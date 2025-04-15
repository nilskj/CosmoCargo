using CosmoCargo.Models;
using CosmoCargo.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CosmoCargo.Endpoints
{
    public static class TollFormEndpoints
    {
        public static async Task<IResult> GetAllTollForms(
            ITollFormService tollFormService)
        {
            var tollForms = await tollFormService.GetAllTollFormsAsync();
            return Results.Ok(tollForms);
        }

        public static async Task<IResult> GetTollFormById(
            Guid id,
            ITollFormService tollFormService,
            ClaimsPrincipal user)
        {
            var tollForm = await tollFormService.GetTollFormByIdAsync(id);
            if (tollForm == null)
            {
                return Results.NotFound();
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            if (role == UserRole.Admin.ToString())
            {
                return Results.Ok(tollForm);
            }
            else if (role == UserRole.Pilot.ToString() && tollForm.Shipment.PilotId == userId)
            {
                return Results.Ok(tollForm);
            }
            else if (role == UserRole.Customer.ToString() && tollForm.SubmittedById == userId)
            {
                return Results.Ok(tollForm);
            }

            return Results.Forbid();
        }

        public static async Task<IResult> CreateTollForm(
            CreateTollFormRequest request,
            ITollFormService tollFormService,
            IShipmentService shipmentService,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var shipment = await shipmentService.GetShipmentByIdAsync(request.ShipmentId);
            if (shipment == null)
            {
                return Results.NotFound("Shipment not found");
            }

            if (shipment.CustomerId != userId)
            {
                return Results.Forbid();
            }

            var existingTollForm = await tollFormService.GetTollFormByShipmentIdAsync(request.ShipmentId);
            if (existingTollForm != null)
            {
                return Results.BadRequest("A toll form already exists for this shipment");
            }

            try
            {
                var tollForm = new TollForm
                {
                    ShipmentId = request.ShipmentId,
                    ContainsLifeforms = request.ContainsLifeforms,
                    LifeformType = request.LifeformType,
                    IsPlasmaActive = request.IsPlasmaActive,
                    PlasmaStabilityLevel = request.PlasmaStabilityLevel,
                    OriginPlanetLawsConfirmed = request.OriginPlanetLawsConfirmed,
                    QuarantineRequired = request.QuarantineRequired,
                    CustomsNotes = request.CustomsNotes,
                    SubmittedById = userId
                };

                var createdTollForm = await tollFormService.CreateTollFormAsync(tollForm);
                return Results.Created($"/api/tollforms/{createdTollForm.Id}", createdTollForm);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        public static async Task<IResult> ReviewTollForm(
            Guid id,
            ReviewTollFormRequest request,
            ITollFormService tollFormService,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var updatedTollForm = await tollFormService.ReviewTollFormAsync(
                id, 
                request.IsApproved, 
                request.ReviewNotes, 
                userId);

            if (updatedTollForm == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(updatedTollForm);
        }
    }

    public class CreateTollFormRequest
    {
        public Guid ShipmentId { get; set; }
        public bool ContainsLifeforms { get; set; }
        public string? LifeformType { get; set; }
        public bool IsPlasmaActive { get; set; }
        public int? PlasmaStabilityLevel { get; set; }
        public bool OriginPlanetLawsConfirmed { get; set; }
        public bool QuarantineRequired { get; set; }
        public string? CustomsNotes { get; set; }
    }

    public class ReviewTollFormRequest
    {
        public bool IsApproved { get; set; }
        public string? ReviewNotes { get; set; }
    }
} 