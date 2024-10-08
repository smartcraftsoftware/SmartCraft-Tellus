using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations.Vehicle
{
    /// <inheritdoc />
    public partial class UpdatedVehicleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusReports_Vehicles_VehicleId",
                table: "StatusReports");

            migrationBuilder.DropIndex(
                name: "IX_StatusReports_VehicleId",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "StatusReports");

            migrationBuilder.AddColumn<string>(
                name: "EngineType",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GearBoxType",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfAxles",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionDateId",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TachographType",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VehicleProductionDate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleProductionDate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ProductionDateId",
                table: "Vehicles",
                column: "ProductionDateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleProductionDate_ProductionDateId",
                table: "Vehicles",
                column: "ProductionDateId",
                principalTable: "VehicleProductionDate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleProductionDate_ProductionDateId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleProductionDate");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ProductionDateId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EngineType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "GearBoxType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NoOfAxles",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ProductionDateId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TachographType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Vehicles");

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "StatusReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusReports_VehicleId",
                table: "StatusReports",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusReports_Vehicles_VehicleId",
                table: "StatusReports",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }
    }
}
