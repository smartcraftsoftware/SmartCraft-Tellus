using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations.Vehicle
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EsgVehicleReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StopTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EsgVehicleReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "text", nullable: true),
                    CustomerVehicleName = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    Brand = table.Column<string>(type: "text", nullable: true),
                    PossibleFuelTypes = table.Column<List<string>>(type: "text[]", nullable: true),
                    EmissionLevel = table.Column<string>(type: "text", nullable: true),
                    TotalFuelTankVolume = table.Column<double>(type: "double precision", nullable: true),
                    TotalFuelTankCapacityGaseous = table.Column<double>(type: "double precision", nullable: true),
                    TotalBatteryPackCapacity = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "text", nullable: true),
                    TotalEngineTime = table.Column<double>(type: "double precision", nullable: true),
                    EngineRunningTime = table.Column<string>(type: "text", nullable: true),
                    AvgSpeed = table.Column<double>(type: "double precision", nullable: true),
                    AvgFuelConsumption = table.Column<double>(type: "double precision", nullable: true),
                    AvgElectricEnergyConsumption = table.Column<double>(type: "double precision", nullable: true),
                    TotalFuelConsumption = table.Column<double>(type: "double precision", nullable: true),
                    FuelConsumptionPerHour = table.Column<double>(type: "double precision", nullable: true),
                    Co2Emissions = table.Column<double>(type: "double precision", nullable: true),
                    Co2Saved = table.Column<double>(type: "double precision", nullable: true),
                    TotalDistance = table.Column<double>(type: "double precision", nullable: true),
                    TotalGasUsed = table.Column<double>(type: "double precision", nullable: true),
                    EsgVehicleReportId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleEvaluations_EsgVehicleReports_EsgVehicleReportId",
                        column: x => x.EsgVehicleReportId,
                        principalTable: "EsgVehicleReports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StatusReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StopTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HrTotalVehicleDistance = table.Column<double>(type: "double precision", nullable: true),
                    TotalEngineHours = table.Column<double>(type: "double precision", nullable: true),
                    TotalElectricMotorHours = table.Column<double>(type: "double precision", nullable: true),
                    EngineTotalFuelUsed = table.Column<double>(type: "double precision", nullable: true),
                    TotalGaseousFuelUsed = table.Column<double>(type: "double precision", nullable: true),
                    TotalElectricEnergyUsed = table.Column<double>(type: "double precision", nullable: true),
                    FuelConsumptionDuringCruiseActive = table.Column<double>(type: "double precision", nullable: true),
                    FuelConsumptionDuringCruiseActiveGaseous = table.Column<double>(type: "double precision", nullable: true),
                    Ignition = table.Column<string>(type: "text", nullable: true),
                    EngineSpeed = table.Column<double>(type: "double precision", nullable: true),
                    ElectricMotorSpeed = table.Column<double>(type: "double precision", nullable: true),
                    FuelType = table.Column<string>(type: "text", nullable: true),
                    FuelLevel1 = table.Column<double>(type: "double precision", nullable: true),
                    FuelLevel2 = table.Column<double>(type: "double precision", nullable: true),
                    CatalystFuelLevel = table.Column<double>(type: "double precision", nullable: true),
                    TotalToEmpty = table.Column<double>(type: "double precision", nullable: true),
                    FuelToEmpty = table.Column<double>(type: "double precision", nullable: true),
                    GasToEmpty = table.Column<double>(type: "double precision", nullable: true),
                    BatteryPack = table.Column<double>(type: "double precision", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusReports_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusReports_VehicleId",
                table: "StatusReports",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleEvaluations_EsgVehicleReportId",
                table: "VehicleEvaluations",
                column: "EsgVehicleReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusReports");

            migrationBuilder.DropTable(
                name: "VehicleEvaluations");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "EsgVehicleReports");
        }
    }
}
