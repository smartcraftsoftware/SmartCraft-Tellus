﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartCraft.Core.Tellus.Infrastructure.Context;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations.Vehicle
{
    [DbContext(typeof(VehicleContext))]
    [Migration("20240701133424_create-interval-status-report")]
    partial class createintervalstatusreport
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.EsgVehicleReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LastUpdatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StopTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("EsgVehicleReports");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.IntervalStatusReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("EndDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("EngineTotalFuelUsed")
                        .HasColumnType("double precision");

                    b.Property<double?>("HrTotalVehicleDistance")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LastUpdatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("TotalElectricEnergyUsed")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalElectricMotorHours")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalEngineHours")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalGaseousFuelUsed")
                        .HasColumnType("double precision");

                    b.Property<string>("Vin")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("IntervalReports");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.StatusReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("BatteryPack")
                        .HasColumnType("double precision");

                    b.Property<double?>("CatalystFuelLevel")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("ElectricMotorSpeed")
                        .HasColumnType("double precision");

                    b.Property<double?>("EngineSpeed")
                        .HasColumnType("double precision");

                    b.Property<double?>("EngineTotalFuelUsed")
                        .HasColumnType("double precision");

                    b.Property<double?>("FuelConsumptionDuringCruiseActive")
                        .HasColumnType("double precision");

                    b.Property<double?>("FuelConsumptionDuringCruiseActiveGaseous")
                        .HasColumnType("double precision");

                    b.Property<double?>("FuelLevel1")
                        .HasColumnType("double precision");

                    b.Property<double?>("FuelLevel2")
                        .HasColumnType("double precision");

                    b.Property<double?>("FuelToEmpty")
                        .HasColumnType("double precision");

                    b.Property<string>("FuelType")
                        .HasColumnType("text");

                    b.Property<double?>("GasToEmpty")
                        .HasColumnType("double precision");

                    b.Property<double?>("HrTotalVehicleDistance")
                        .HasColumnType("double precision");

                    b.Property<string>("Ignition")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LastUpdatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ReceivedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StopTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("TotalElectricEnergyUsed")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalElectricMotorHours")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalEngineHours")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalGaseousFuelUsed")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalToEmpty")
                        .HasColumnType("double precision");

                    b.Property<Guid?>("VehicleId")
                        .HasColumnType("uuid");

                    b.Property<string>("Vin")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("StatusReports");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Brand")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("CustomerVehicleName")
                        .HasColumnType("text");

                    b.Property<string>("EmissionLevel")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LastUpdatedBy")
                        .HasColumnType("uuid");

                    b.Property<List<string>>("PossibleFuelTypes")
                        .HasColumnType("text[]");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("text");

                    b.Property<double?>("TotalBatteryPackCapacity")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalFuelTankCapacityGaseous")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalFuelTankVolume")
                        .HasColumnType("double precision");

                    b.Property<string>("Vin")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.VehicleEvaluation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("AvgElectricEnergyConsumption")
                        .HasColumnType("double precision");

                    b.Property<double?>("AvgFuelConsumption")
                        .HasColumnType("double precision");

                    b.Property<double?>("AvgSpeed")
                        .HasColumnType("double precision");

                    b.Property<double?>("Co2Emissions")
                        .HasColumnType("double precision");

                    b.Property<double?>("Co2Saved")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("EngineRunningTime")
                        .HasColumnType("text");

                    b.Property<Guid?>("EsgVehicleReportId")
                        .HasColumnType("uuid");

                    b.Property<double?>("FuelConsumptionPerHour")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LastUpdatedBy")
                        .HasColumnType("uuid");

                    b.Property<double?>("TotalDistance")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalEngineTime")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalFuelConsumption")
                        .HasColumnType("double precision");

                    b.Property<double?>("TotalGasUsed")
                        .HasColumnType("double precision");

                    b.Property<string>("Vin")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EsgVehicleReportId");

                    b.ToTable("VehicleEvaluations");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.StatusReport", b =>
                {
                    b.HasOne("SmartCraft.Core.Tellus.Infrastructure.Models.Vehicle", null)
                        .WithMany("StatusReports")
                        .HasForeignKey("VehicleId");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.VehicleEvaluation", b =>
                {
                    b.HasOne("SmartCraft.Core.Tellus.Infrastructure.Models.EsgVehicleReport", null)
                        .WithMany("VehicleEvaluations")
                        .HasForeignKey("EsgVehicleReportId");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.EsgVehicleReport", b =>
                {
                    b.Navigation("VehicleEvaluations");
                });

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.Vehicle", b =>
                {
                    b.Navigation("StatusReports");
                });
#pragma warning restore 612, 618
        }
    }
}
