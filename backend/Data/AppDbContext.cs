using CosmoCargo.Model;
using Microsoft.EntityFrameworkCore;

namespace CosmoCargo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplySnakeCaseNamingConvention();

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Experience);
                entity.Property(e => e.IsActive);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasMany(u => u.CustomerShipments)
                    .WithOne(s => s.Customer)
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.PilotShipments)
                    .WithOne(s => s.Pilot)
                    .HasForeignKey(s => s.PilotId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                // Configure owned entity types
                entity.OwnsOne(e => e.Sender, sender =>
                {
                    sender.Property(s => s.Name).IsRequired();
                    sender.Property(s => s.Email).IsRequired();
                    sender.Property(s => s.Planet).IsRequired();
                    sender.Property(s => s.Station).IsRequired();
                });
                
                entity.OwnsOne(e => e.Receiver, receiver =>
                {
                    receiver.Property(r => r.Name).IsRequired();
                    receiver.Property(r => r.Email).IsRequired();
                    receiver.Property(r => r.Planet).IsRequired();
                    receiver.Property(r => r.Station).IsRequired();
                });
                
                // Package information
                entity.Property(e => e.Weight).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.Description);
                entity.Property(e => e.HasInsurance).IsRequired();
                
                // Status and tracking
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });
        }
    }
}
