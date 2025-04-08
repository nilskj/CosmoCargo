namespace CosmoCargo.Models
{
    public class TollForm
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public bool ContainsLifeforms { get; set; }
        public string? LifeformType { get; set; }
        public bool IsPlasmaActive { get; set; }
        public int? PlasmaStabilityLevel { get; set; }
        public bool OriginPlanetLawsConfirmed { get; set; }
        public bool QuarantineRequired { get; set; }
        public string? CustomsNotes { get; set; }
        public Guid SubmittedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsApproved { get; set; }
        public string? ReviewNotes { get; set; }
        public Guid? ReviewedById { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public virtual Shipment Shipment { get; set; } = null!;
        public virtual User SubmittedBy { get; set; } = null!;
    }
} 