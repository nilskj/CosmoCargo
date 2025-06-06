﻿// <auto-generated />
using System;
using System.Collections.Generic;
using CosmoCargo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CosmoCargo.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250429132240_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CosmoCargo.Model.Shipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customer_id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("HasInsurance")
                        .HasColumnType("boolean")
                        .HasColumnName("has_insurance");

                    b.Property<Guid?>("PilotId")
                        .HasColumnType("uuid")
                        .HasColumnName("pilot_id");

                    b.Property<string>("Priority")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("priority");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<decimal>("Weight")
                        .HasColumnType("numeric")
                        .HasColumnName("weight");

                    b.ComplexProperty<Dictionary<string, object>>("Receiver", "CosmoCargo.Model.Shipment.Receiver#ShipmentContact", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("receiver_email");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("receiver_name");

                            b1.Property<string>("Planet")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("receiver_planet");

                            b1.Property<string>("Station")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("receiver_station");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Sender", "CosmoCargo.Model.Shipment.Sender#ShipmentContact", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("sender_email");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("sender_name");

                            b1.Property<string>("Planet")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("sender_planet");

                            b1.Property<string>("Station")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("sender_station");
                        });

                    b.HasKey("Id")
                        .HasName("p_k_shipments");

                    b.HasIndex("CustomerId");

                    b.HasIndex("PilotId");

                    b.ToTable("shipments", (string)null);
                });

            modelBuilder.Entity("CosmoCargo.Model.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Experience")
                        .HasColumnType("text")
                        .HasColumnName("experience");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("p_k_users");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("users");
                });

            modelBuilder.Entity("CosmoCargo.Model.Shipment", b =>
                {
                    b.HasOne("CosmoCargo.Model.User", "Customer")
                        .WithMany("CustomerShipments")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CosmoCargo.Model.User", "Pilot")
                        .WithMany("PilotShipments")
                        .HasForeignKey("PilotId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Customer");

                    b.Navigation("Pilot");
                });

            modelBuilder.Entity("CosmoCargo.Model.User", b =>
                {
                    b.Navigation("CustomerShipments");

                    b.Navigation("PilotShipments");
                });
#pragma warning restore 612, 618
        }
    }
}
