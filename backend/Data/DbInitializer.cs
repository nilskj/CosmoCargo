using CosmoCargo.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Npgsql;

namespace CosmoCargo.Data
{
    public static class DbInitializer
    {
        private static int TotalCustomers = Random.Shared.Next(200_000, 300_000);
        private static int TotalPilots = Random.Shared.Next(100_000, 200_000);
        private static int TotalAdmins = Random.Shared.Next(50_000, 100_000);
        private static int TotalShipments = Random.Shared.Next(6_000_000, 8_000_000);

        private static readonly Random _random = new Random();
        private static readonly ConcurrentDictionary<string, byte> _takenEmails = new ConcurrentDictionary<string, byte>();
        private static Guid DemoCustomerId;
        private static Guid DemoPilotId;
        private static Guid DemoAdminId;
        private const int BatchSize = 100_000;
        private const int MaxRetries = 5;
        private const int MaxDegreeOfParallelism = 5;
        private const int DeadlockRetryDelay = 2000;
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

        private static string GenerateUniqueEmail(string firstName, string lastName, string domain)
        {
            var baseEmail = $"{firstName.ToLower()}.{lastName.ToLower()}";
            var email = $"{baseEmail}@{domain}";
            var counter = 2;

            while (!_takenEmails.TryAdd(email, 0))
            {
                email = $"{baseEmail}.{counter}@{domain}";
                counter++;
            }

            return email;
        }

        private static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Apply migrations
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                    ""MigrationId"" character varying(150) NOT NULL,
                    ""ProductVersion"" character varying(32) NOT NULL,
                    CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                );
            ");
            await context.Database.MigrateAsync();

            if (await context.Users.AnyAsync())
                return;

            Log("DATABASE: Start seeding");

            await SeedDemoUsers(serviceProvider);
            await SeedCustomers(serviceProvider);
            await SeedPilots(serviceProvider);
            await SeedAdmins(serviceProvider);
            await SeedShipments(serviceProvider);

            Log("DATABASE: Seeding completed!");
        }

        private static async Task BulkInsertUsersAsync(AppDbContext context, List<User> users)
        {
            var connection = (NpgsqlConnection)context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var writer = await connection.BeginBinaryImportAsync(
                "COPY users (id, name, email, password_hash, role, experience, is_active, created_at) FROM STDIN (FORMAT BINARY)");

            foreach (var user in users)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(user.Id);
                await writer.WriteAsync(user.Name);
                await writer.WriteAsync(user.Email);
                await writer.WriteAsync(user.PasswordHash);
                await writer.WriteAsync((int)user.Role);
                await writer.WriteAsync(user.Experience);
                await writer.WriteAsync(user.IsActive);
                await writer.WriteAsync(user.CreatedAt);
            }

            await writer.CompleteAsync();
        }

        private static async Task BulkInsertShipmentsAsync(AppDbContext context, List<Shipment> shipments)
        {
            var connection = (NpgsqlConnection)context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var writer = await connection.BeginBinaryImportAsync(
                "COPY shipments (id, customer_id, pilot_id, sender_name, sender_email, sender_planet, sender_station, receiver_name, receiver_email, receiver_planet, receiver_station, weight, category, priority, description, has_insurance, status, scheduled_date, created_at, updated_at) FROM STDIN (FORMAT BINARY)");

            foreach (var shipment in shipments)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(shipment.Id);
                await writer.WriteAsync(shipment.CustomerId);
                await writer.WriteAsync(shipment.PilotId);
                await writer.WriteAsync(shipment.Sender.Name);
                await writer.WriteAsync(shipment.Sender.Email);
                await writer.WriteAsync(shipment.Sender.Planet);
                await writer.WriteAsync(shipment.Sender.Station);
                await writer.WriteAsync(shipment.Receiver.Name);
                await writer.WriteAsync(shipment.Receiver.Email);
                await writer.WriteAsync(shipment.Receiver.Planet);
                await writer.WriteAsync(shipment.Receiver.Station);
                await writer.WriteAsync(shipment.Weight);
                await writer.WriteAsync(shipment.Category);
                await writer.WriteAsync(shipment.Priority);
                await writer.WriteAsync(shipment.Description);
                await writer.WriteAsync(shipment.HasInsurance);
                await writer.WriteAsync((int)shipment.Status);
                await writer.WriteAsync(shipment.ScheduledDate);
                await writer.WriteAsync(shipment.CreatedAt);
                await writer.WriteAsync(shipment.UpdatedAt);
            }

            await writer.CompleteAsync();
        }

        private static async Task SeedDemoUsers(IServiceProvider serviceProvider)
        {
            Log("Starting to seed demo customers");

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            DemoCustomerId = Guid.NewGuid();
            DemoPilotId = Guid.NewGuid();
            DemoAdminId = Guid.NewGuid();

            var demoUsers = new List<User>
            {
                new User
                {
                    Id = DemoCustomerId,
                    Name = "Demo Customer",
                    Email = GenerateUniqueEmail("demo", "customer", "example.com"),
                    PasswordHash = Utils.Crypto.HashPassword("mKv2P8dXrL9F"),
                    Role = UserRole.Customer,
                    Experience = null,
                    IsActive = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 365))
                },
                new User
                {
                    Id = DemoPilotId,
                    Name = "Demo Pilot",
                    Email = GenerateUniqueEmail("demo", "pilot", "example.com"),
                    PasswordHash = Utils.Crypto.HashPassword("zH7yB3tR5wQ9s"),
                    Role = UserRole.Pilot,
                    Experience = null,
                    IsActive = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 365))
                },
                new User
                {
                    Id = DemoAdminId,
                    Name = "Demo Admin",
                    Email = GenerateUniqueEmail("demo", "admin", "example.com"),
                    PasswordHash = Utils.Crypto.HashPassword("eT4xD6cV2gN8p"),
                    Role = UserRole.Admin,
                    Experience = null,
                    IsActive = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 365))
                },
            };

            await context.Users.AddRangeAsync(demoUsers);
            await context.SaveChangesAsync();

            Log("Demo customers seeded!");
        }

        private static async Task SeedCustomers(IServiceProvider serviceProvider)
        {
            Log($"Starting to seed {TotalCustomers:N0} customers");
            var batches = (int)Math.Ceiling((double)TotalCustomers / BatchSize);
            var options = new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
            await Parallel.ForEachAsync(Enumerable.Range(0, batches), options, async (batch, ct) =>
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var customers = new List<User>(BatchSize);
                var now = DateTime.UtcNow;
                var batchSize = Math.Min(BatchSize, TotalCustomers - batch * BatchSize);

                for (int i = 0; i < batchSize; i++)
                {
                    var firstName = _firstNames[_random.Next(_firstNames.Length)];
                    var lastName = _lastNames[_random.Next(_lastNames.Length)];
                    var experience = $"{_experienceLevels[_random.Next(_experienceLevels.Length)]} in {_industries[_random.Next(_industries.Length)]}";
                    var userNumber = batch * BatchSize + i + 1;

                    customers.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{firstName} {lastName}",
                        Email = GenerateUniqueEmail(firstName, lastName, "example.com"),
                        PasswordHash = Utils.Crypto.HashPassword($"Customer{userNumber}"),
                        Role = UserRole.Customer,
                        Experience = experience,
                        IsActive = _random.Next(100) < 95,
                        CreatedAt = now.AddDays(-_random.Next(0, 365))
                    });
                }

                for (int retry = 0; retry < MaxRetries; retry++)
                {
                    try
                    {
                        using var transaction = await context.Database.BeginTransactionAsync();
                        try
                        {
                            await BulkInsertUsersAsync(context, customers);
                            await transaction.CommitAsync();
                            break;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                    catch (Exception ex) when (IsDeadlockException(ex) && retry < MaxRetries - 1)
                    {
                        Log($"> Deadlock detected on customer batch {batch + 1}, retry {retry + 1}");
                        await Task.Delay(DeadlockRetryDelay * (retry + 1));
                    }
                    catch (Exception ex) when (retry < MaxRetries - 1)
                    {
                        Log($"> Retry {retry + 1} for customer batch {batch + 1} due to: {ex.Message}");
                        await Task.Delay(1000 * (retry + 1));
                    }
                }

                Log($"> Processed {Math.Min((batch + 1) * BatchSize, TotalCustomers):N0} customers");
            });

            Log("Customer seeding completed!");
        }

        private static bool IsDeadlockException(Exception ex)
        {
            return ex is PostgresException pgEx && pgEx.SqlState == "40P01";
        }

        private static async Task SeedPilots(IServiceProvider serviceProvider)
        {
            Log($"Starting to seed {TotalPilots:N0} pilots");
            var batches = (int)Math.Ceiling((double)TotalPilots / BatchSize);
            var options = new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
            await Parallel.ForEachAsync(Enumerable.Range(0, batches), options, async (batch, ct) =>
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pilots = new List<User>(BatchSize);
                var now = DateTime.UtcNow;
                var batchSize = Math.Min(BatchSize, TotalPilots - batch * BatchSize);

                for (int i = 0; i < batchSize; i++)
                {
                    var firstName = _firstNames[_random.Next(_firstNames.Length)];
                    var lastName = _lastNames[_random.Next(_lastNames.Length)];
                    var experience = $"{_experienceLevels[_random.Next(_experienceLevels.Length)]} of space piloting";
                    var userNumber = batch * BatchSize + i + 1;

                    pilots.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{firstName} {lastName}",
                        Email = GenerateUniqueEmail(firstName, lastName, "cosmocargo.com"),
                        PasswordHash = Utils.Crypto.HashPassword($"Pilot{userNumber}"),
                        Role = UserRole.Pilot,
                        Experience = experience,
                        IsActive = _random.Next(100) < 90,
                        CreatedAt = now.AddDays(-_random.Next(0, 365))
                    });
                }

                for (int retry = 0; retry < MaxRetries; retry++)
                {
                    try
                    {
                        using var transaction = await context.Database.BeginTransactionAsync();
                        try
                        {
                            await BulkInsertUsersAsync(context, pilots);
                            await transaction.CommitAsync();
                            break;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                    catch (Exception ex) when (IsDeadlockException(ex) && retry < MaxRetries - 1)
                    {
                        Log($"> Deadlock detected on pilot batch {batch + 1}, retry {retry + 1}");
                        await Task.Delay(DeadlockRetryDelay * (retry + 1));
                    }
                    catch (Exception ex) when (retry < MaxRetries - 1)
                    {
                        Log($"> Retry {retry + 1} for pilot batch {batch + 1} due to: {ex.Message}");
                        await Task.Delay(1000 * (retry + 1));
                    }
                }

                Log($"> Processed {Math.Min((batch + 1) * BatchSize, TotalPilots):N0} pilots");
            });

            Log("Pilot seeding completed!");
        }

        private static async Task SeedAdmins(IServiceProvider serviceProvider)
        {
            Log($"Starting to seed {TotalAdmins:N0} admins");
            var batches = (int)Math.Ceiling((double)TotalAdmins / BatchSize);
            var options = new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
            await Parallel.ForEachAsync(Enumerable.Range(0, batches), options, async (batch, ct) =>
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var admins = new List<User>(BatchSize);
                var now = DateTime.UtcNow;
                var batchSize = Math.Min(BatchSize, TotalAdmins - batch * BatchSize);

                for (int i = 0; i < batchSize; i++)
                {
                    var firstName = _firstNames[_random.Next(_firstNames.Length)];
                    var lastName = _lastNames[_random.Next(_lastNames.Length)];
                    var experience = $"{_experienceLevels[_random.Next(_experienceLevels.Length)]} in space logistics management";
                    var userNumber = batch * BatchSize + i + 1;

                    admins.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{firstName} {lastName}",
                        Email = GenerateUniqueEmail(firstName, lastName, "cosmocargo.com"),
                        PasswordHash = Utils.Crypto.HashPassword($"Admin{userNumber}"),
                        Role = UserRole.Admin,
                        Experience = experience,
                        IsActive = _random.Next(100) < 98,
                        CreatedAt = now.AddDays(-_random.Next(0, 365))
                    });
                }

                for (int retry = 0; retry < MaxRetries; retry++)
                {
                    try
                    {
                        using var transaction = await context.Database.BeginTransactionAsync();
                        try
                        {
                            await BulkInsertUsersAsync(context, admins);
                            await transaction.CommitAsync();
                            break;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                    catch (Exception ex) when (IsDeadlockException(ex) && retry < MaxRetries - 1)
                    {
                        Log($"> Deadlock detected on admin batch {batch + 1}, retry {retry + 1}");
                        await Task.Delay(DeadlockRetryDelay * (retry + 1));
                    }
                    catch (Exception ex) when (retry < MaxRetries - 1)
                    {
                        Log($"> Retry {retry + 1} for admin batch {batch + 1} due to: {ex.Message}");
                        await Task.Delay(1000 * (retry + 1));
                    }
                }

                Log($"> Processed {Math.Min((batch + 1) * BatchSize, TotalAdmins):N0} admins");
            });

            Log("Admin seeding completed!");
        }

        private static async Task SeedShipments(IServiceProvider serviceProvider)
        {
            Log($"Starting to seed {TotalShipments:N0} shipments");

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var customerIds = await context.Users
                .Where(u => u.Role == UserRole.Customer)
                .Select(u => u.Id)
                .ToListAsync();

            var pilotIds = await context.Users
                .Where(u => u.Role == UserRole.Pilot)
                .Select(u => u.Id)
                .ToListAsync();

            var statuses = Enum.GetValues<ShipmentStatus>();
            var priorities = new[] { "Low", "Normal", "High", "Urgent" };
            
            var batches = (int)Math.Ceiling((double)TotalShipments / BatchSize);
            var options = new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
            await Parallel.ForEachAsync(Enumerable.Range(0, batches), options, async (batch, ct) =>
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var shipments = new List<Shipment>(BatchSize);
                var now = DateTime.UtcNow;
                var batchSize = Math.Min(BatchSize, TotalShipments - batch * BatchSize);

                for (int i = 0; i < batchSize; i++)
                {
                    var scheduledDate = now.AddDays(_random.Next(-365, 365));
                    var createdAt = scheduledDate.AddDays(-_random.Next(1, 30));
                    var updatedAt = createdAt.AddDays(_random.Next(0, 30));
                    var status = statuses[_random.Next(statuses.Length)];

                    shipments.Add(new Shipment
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = Random.Shared.Next(100) < 5 ? DemoCustomerId : customerIds[_random.Next(customerIds.Count)],
                        PilotId = status == ShipmentStatus.Assigned 
                            || status == ShipmentStatus.InTransit 
                            || status == ShipmentStatus.Delivered ? (Random.Shared.Next(100) < 5 ? DemoPilotId : pilotIds[_random.Next(pilotIds.Count)]) : null,
                        
                        // Sender information
                        Sender = new ShipmentContact
                        {
                            Name = $"{_firstNames[_random.Next(_firstNames.Length)]} {_lastNames[_random.Next(_lastNames.Length)]}",
                            Email = GenerateUniqueEmail("sender", "shipment", "example.com"),
                            Planet = _origins[_random.Next(_origins.Length)].Split(',')[0].Trim(),
                            Station = _origins[_random.Next(_origins.Length)].Split(',')[1].Trim()
                        },
                        
                        // Receiver information
                        Receiver = new ShipmentContact
                        {
                            Name = $"{_firstNames[_random.Next(_firstNames.Length)]} {_lastNames[_random.Next(_lastNames.Length)]}",
                            Email = GenerateUniqueEmail("receiver", "shipment", "example.com"),
                            Planet = _destinations[_random.Next(_destinations.Length)].Split(',')[0].Trim(),
                            Station = _destinations[_random.Next(_destinations.Length)].Split(',')[1].Trim()
                        },
                        
                        // Package information
                        Weight = _random.Next(50, 1000),
                        Category = _cargoTypes[_random.Next(_cargoTypes.Length)],
                        Priority = priorities[_random.Next(priorities.Length)],
                        Description = $"Sample shipment #{i + 1}",
                        HasInsurance = _random.Next(100) < 70, // 70% chance of having insurance
                        
                        // Status and tracking
                        Status = status,
                        ScheduledDate = scheduledDate,
                        CreatedAt = createdAt,
                        UpdatedAt = updatedAt
                    });
                }

                for (int retry = 0; retry < MaxRetries; retry++)
                {
                    try
                    {
                        using var transaction = await context.Database.BeginTransactionAsync();
                        try
                        {
                            await BulkInsertShipmentsAsync(context, shipments);
                            await transaction.CommitAsync();
                            break;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                    catch (Exception ex) when (IsDeadlockException(ex) && retry < MaxRetries - 1)
                    {
                        Log($"> Deadlock detected on shipment batch {batch + 1}, retry {retry + 1}");
                        await Task.Delay(DeadlockRetryDelay * (retry + 1));
                    }
                    catch (Exception ex) when (retry < MaxRetries - 1)
                    {
                        Log($"> Retry {retry + 1} for shipment batch {batch + 1} due to: {ex.Message}");
                        await Task.Delay(1000 * (retry + 1));
                    }
                }

                Log($"> Processed {Math.Min((batch + 1) * BatchSize, TotalShipments):N0} shipments");
            });

            Log("Shipment seeding completed!");
        }
    }
}
