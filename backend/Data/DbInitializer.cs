using CosmoCargo.Models;
using System.Security.Cryptography;
using System.Text;

namespace CosmoCargo.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin Adminsson",
                    Email = "admin@cosmocargo.com",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Pilot Pilotsson",
                    Email = "pilot@cosmocargo.com",
                    PasswordHash = HashPassword("Pilot123!"),
                    Role = UserRole.Pilot,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Kund Kundsson",
                    Email = "kund@example.com",
                    PasswordHash = HashPassword("Kund123!"),
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            var shipments = new List<Shipment>
            {
                new Shipment
                {
                    Id = Guid.NewGuid(),
                    CustomerId = users.First(u => u.Role == UserRole.Customer).Id,
                    PilotId = users.First(u => u.Role == UserRole.Pilot).Id,
                    Origin = "Jorden",
                    Destination = "Mars",
                    Weight = 150.5m,
                    Category = "Elektronik",
                    Priority = "Hög",
                    Status = ShipmentStatus.Pending,
                    RiskLevel = RiskLevel.Low,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Shipment
                {
                    Id = Guid.NewGuid(),
                    CustomerId = users.First(u => u.Role == UserRole.Customer).Id,
                    PilotId = users.First(u => u.Role == UserRole.Pilot).Id,
                    Origin = "Mars",
                    Destination = "Jupiter",
                    Weight = 75.2m,
                    Category = "Livsmedel",
                    Priority = "Normal",
                    Status = ShipmentStatus.Pending,
                    RiskLevel = RiskLevel.Low,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Shipments.AddRange(shipments);
            context.SaveChanges();

            var tollForms = new List<TollForm>
            {
                new TollForm
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[0].Id,
                    ContainsLifeforms = false,
                    IsPlasmaActive = true,
                    PlasmaStabilityLevel = 8,
                    OriginPlanetLawsConfirmed = true,
                    QuarantineRequired = false,
                    CustomsNotes = "Standard plasma-material för forskningsändamål",
                    SubmittedById = users.First(u => u.Role == UserRole.Customer).Id,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.TollForms.AddRange(tollForms);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 