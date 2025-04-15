using CosmoCargo.Models;
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
        public DbSet<TollForm> TollForms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Customer)
                .WithMany(u => u.CustomerShipments)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Pilot)
                .WithMany(u => u.PilotShipments)
                .HasForeignKey(s => s.PilotId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<TollForm>()
                .HasOne(t => t.Shipment)
                .WithOne(s => s.TollForm)
                .HasForeignKey<TollForm>(t => t.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Shipment>()
                .Property(s => s.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Shipment>()
                .Property(s => s.RiskLevel)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
} 