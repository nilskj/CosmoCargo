namespace CosmoCargo.Models
{
    public enum ShipmentStatus
    {
        Pending,
        InProgress,
        Delivered,
        LostInBlackHole
    }

    public enum RiskLevel
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class Shipment
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? PilotId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public ShipmentStatus Status { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User Customer { get; set; } = null!;
        public virtual User? Pilot { get; set; }
        public virtual TollForm? TollForm { get; set; }
    }
} 