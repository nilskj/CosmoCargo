using CosmoCargo.Model;
using System.Security.Cryptography;
using System.Text;

namespace CosmoCargo.Data
{
    public static class DbInitializer
    {
        private static readonly Random _random = new Random();
        private static readonly string[] _origins = new[]
        {
            "Stockholm, Sweden", "Gothenburg, Sweden", "Malmö, Sweden",
            "Jorden, Alpha Station", "Jorden, Beta Station", "Jorden, Gamma Station",
            "Mars, Olympus Station", "Mars Base One", "Lunar Colony Alpha",
            "Titan Research Station", "Europa, Ice Harbor", "Ganymede Outpost",
            "Callisto Mining Facility", "Io Research Base", "Enceladus Station"
        };

        private static readonly string[] _destinations = new[]
        {
            "Mars, Olympus Station", "Mars Base One", "Lunar Colony Alpha",
            "Titan Research Station", "Europa, Ice Harbor", "Ganymede Outpost",
            "Callisto Mining Facility", "Io Research Base", "Enceladus Station",
            "Triton Research Base", "Pluto Station", "Charon Outpost",
            "Ceres Mining Facility", "Vesta Research Station", "Pallas Colony"
        };

        private static readonly string[] _cargoTypes = new[]
        {
            "Scientific Equipment", "Medical Supplies", "Construction Materials",
            "Food Supplies", "Mining Equipment", "Research Samples",
            "Spare Parts", "Personal Effects", "Industrial Machinery",
            "Water Supplies", "Oxygen Tanks", "Fuel Cells",
            "Electronics", "Raw Materials", "Agricultural Products"
        };

        private static readonly string[] _firstNames = new[]
        {
            "Erik", "Anna", "Maria", "Johan", "Anders", "Karin", "Lars", "Sofia",
            "Mikael", "Emma", "Per", "Lisa", "Karl", "Eva", "Peter", "Linda",
            "Andreas", "Sara", "Thomas", "Jenny", "Daniel", "Maria", "Fredrik",
            "Emma", "Magnus", "Anna", "Jonas", "Sofia", "Martin", "Karin"
        };

        private static readonly string[] _lastNames = new[]
        {
            "Andersson", "Johansson", "Karlsson", "Nilsson", "Eriksson", "Larsson",
            "Olsson", "Persson", "Svensson", "Gustafsson", "Pettersson", "Jonsson",
            "Jansson", "Hansson", "Bengtsson", "Jönsson", "Lindberg", "Jakobsson",
            "Magnusson", "Olofsson", "Lindström", "Lindqvist", "Lindgren", "Axelsson",
            "Berg", "Bergström", "Lundberg", "Lundgren", "Lundqvist", "Mattsson"
        };

        private static readonly string[] _experienceLevels = new[]
        {
            "1 year", "2 years", "3 years", "4 years", "5 years", "6 years",
            "7 years", "8 years", "9 years", "10 years", "11 years", "12 years",
            "13 years", "14 years", "15 years", "16 years", "17 years", "18 years",
            "19 years", "20 years"
        };

        private static readonly string[] _industries = new[]
        {
            "logistics", "space transport", "interplanetary shipping", "cargo management",
            "space logistics", "freight forwarding", "supply chain", "transportation",
            "space operations", "cargo operations", "shipping management", "space navigation",
            "cargo handling", "space logistics management", "interplanetary operations"
        };

        private static readonly IList<string> _takenEmails = [];

        private static string GenerateUniqueEmail(AppDbContext context, string firstName, string lastName, string domain)
        {
            var baseEmail = $"{firstName.ToLower()}.{lastName.ToLower()}";
            var counter = 2;
            var email = $"{baseEmail}@{domain}";

            while (_takenEmails.Contains(email) || context.Users.Any(u => u.Email == email))
            {
                email = $"{baseEmail}.{counter}@{domain}";
                counter++;
            }

            _takenEmails.Add(email);

            return email;
        }

        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            SeedCustomers(context);
            SeedPilots(context);
            SeedAdmins(context);
            SeedShipments(context);
        }

        private static void SeedCustomers(AppDbContext context)
        {
            int totalCustomers = Random.Shared.Next(500_000, 700_000);
            const int batchSize = 1000;
            var batches = totalCustomers / batchSize;

            Console.WriteLine($"Starting to seed {totalCustomers:N0} customers...");

            for (int batch = 0; batch < batches; batch++)
            {
                var customers = new List<User>(batchSize);
                var now = DateTime.UtcNow;

                for (int i = 0; i < batchSize; i++)
                {
                    var firstName = _firstNames[_random.Next(_firstNames.Length)];
                    var lastName = _lastNames[_random.Next(_lastNames.Length)];
                    var experience = $"{_experienceLevels[_random.Next(_experienceLevels.Length)]} in {_industries[_random.Next(_industries.Length)]}";
                    var userNumber = batch * batchSize + i + 1;

                    customers.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{firstName} {lastName}",
                        Email = GenerateUniqueEmail(context, firstName, lastName, "example.com"),
                        PasswordHash = Utils.Crypto.HashPassword($"Customer{userNumber}"),
                        Role = UserRole.Customer,
                        Experience = experience,
                        IsActive = _random.Next(100) < 95, // 95% active
                        CreatedAt = now.AddDays(-_random.Next(0, 365))
                    });
                }

                context.Users.AddRange(customers);
                context.SaveChanges();

                if ((batch + 1) % 10 == 0)
                {
                    Console.WriteLine($"Processed {(batch + 1) * batchSize:N0} customers...");
                }
            }

            Console.WriteLine("Customer seeding completed!");
        }

        private static void SeedPilots(AppDbContext context)
        {
            int totalPilots = Random.Shared.Next(300_000, 400_000);
            const int batchSize = 1000;
            var batches = totalPilots / batchSize;

            Console.WriteLine($"Starting to seed {totalPilots:N0} pilots...");

            for (int batch = 0; batch < batches; batch++)
            {
                var pilots = new List<User>(batchSize);
                var now = DateTime.UtcNow;

                for (int i = 0; i < batchSize; i++)
                {
                    var firstName = _firstNames[_random.Next(_firstNames.Length)];
                    var lastName = _lastNames[_random.Next(_lastNames.Length)];
                    var experience = $"{_experienceLevels[_random.Next(_experienceLevels.Length)]} of space piloting";
                    var userNumber = batch * batchSize + i + 1;
                    var baseEmail = $"pilot{userNumber}";

                    pilots.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{firstName} {lastName}",
                        Email = GenerateUniqueEmail(context, firstName, lastName, "cosmocargo.com"),
                        PasswordHash = Utils.Crypto.HashPassword($"Pilot{userNumber}"),
                        Role = UserRole.Pilot,
                        Experience = experience,
                        IsActive = _random.Next(100) < 90, // 90% active
                        CreatedAt = now.AddDays(-_random.Next(0, 365))
                    });
                }

                context.Users.AddRange(pilots);
                context.SaveChanges();

                if ((batch + 1) % 10 == 0)
                {
                    Console.WriteLine($"Processed {(batch + 1) * batchSize:N0} pilots...");
                }
            }

            Console.WriteLine("Pilot seeding completed!");
        }

        private static void SeedAdmins(AppDbContext context)
        {
            int totalAdmins = Random.Shared.Next(100_000, 200_000);
            const int batchSize = 1000;
            var batches = totalAdmins / batchSize;

            Console.WriteLine($"Starting to seed {totalAdmins:N0} admins...");

            for (int batch = 0; batch < batches; batch++)
            {
                var admins = new List<User>(batchSize);
                var now = DateTime.UtcNow;

                for (int i = 0; i < batchSize; i++)
                {
                    var firstName = _firstNames[_random.Next(_firstNames.Length)];
                    var lastName = _lastNames[_random.Next(_lastNames.Length)];
                    var experience = $"{_experienceLevels[_random.Next(_experienceLevels.Length)]} in space logistics management";
                    var userNumber = batch * batchSize + i + 1;
                    var baseEmail = $"admin{userNumber}";

                    admins.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{firstName} {lastName}",
                        Email = GenerateUniqueEmail(context, firstName, lastName, "cosmocargo.com"),
                        PasswordHash = Utils.Crypto.HashPassword($"Admin{userNumber}"),
                        Role = UserRole.Admin,
                        Experience = experience,
                        IsActive = _random.Next(100) < 98, // 98% active
                        CreatedAt = now.AddDays(-_random.Next(0, 365))
                    });
                }

                context.Users.AddRange(admins);
                context.SaveChanges();

                if ((batch + 1) % 10 == 0)
                {
                    Console.WriteLine($"Processed {(batch + 1) * batchSize:N0} admins...");
                }
            }

            Console.WriteLine("Admin seeding completed!");
        }

        private static void SeedShipments(AppDbContext context)
        {
            var users = context.Users.ToList();
            var customers = users.Where(u => u.Role == UserRole.Customer).ToList();
            var pilots = users.Where(u => u.Role == UserRole.Pilot).ToList();
            var statuses = Enum.GetValues<ShipmentStatus>();
            var riskLevels = Enum.GetValues<RiskLevel>();
            var priorities = new[] { "Low", "Normal", "High", "Urgent" };

            int totalShipments = Random.Shared.Next(12_000_000, 16_000_000);
            const int batchSize = 1000;
            var batches = totalShipments / batchSize;

            Console.WriteLine($"Starting to seed {totalShipments:N0} shipments...");

            for (int batch = 0; batch < batches; batch++)
            {
                var shipments = new List<Shipment>(batchSize);
                var now = DateTime.UtcNow;

                for (int i = 0; i < batchSize; i++)
                {
                    var scheduledDate = now.AddDays(_random.Next(-365, 365));
                    var createdAt = scheduledDate.AddDays(-_random.Next(1, 30));
                    var updatedAt = createdAt.AddDays(_random.Next(0, 30));

                    shipments.Add(new Shipment
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customers[_random.Next(customers.Count)].Id,
                        PilotId = pilots[_random.Next(pilots.Count)].Id,
                        Origin = _origins[_random.Next(_origins.Length)],
                        Destination = _destinations[_random.Next(_destinations.Length)],
                        Weight = _random.Next(50, 1000),
                        Cargo = _cargoTypes[_random.Next(_cargoTypes.Length)],
                        Priority = priorities[_random.Next(priorities.Length)],
                        Status = statuses[_random.Next(statuses.Length)],
                        RiskLevel = riskLevels[_random.Next(riskLevels.Length)],
                        ScheduledDate = scheduledDate,
                        CreatedAt = createdAt,
                        UpdatedAt = updatedAt
                    });
                }

                context.Shipments.AddRange(shipments);
                context.SaveChanges();

                if ((batch + 1) % 10 == 0)
                {
                    Console.WriteLine($"Processed {(batch + 1) * batchSize:N0} shipments...");
                }
            }

            Console.WriteLine("Shipment seeding completed!");
        }
    }
}
