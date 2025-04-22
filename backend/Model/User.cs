using System.Text.Json.Serialization;

namespace CosmoCargo.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }

#region Pilot-specific properties
        public string? Experience { get; set; }
        public bool? IsActive { get; set; }
#endregion
        
        [JsonIgnore]
        public virtual ICollection<Shipment>? CustomerShipments { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Shipment>? PilotShipments { get; set; }
    }
}
