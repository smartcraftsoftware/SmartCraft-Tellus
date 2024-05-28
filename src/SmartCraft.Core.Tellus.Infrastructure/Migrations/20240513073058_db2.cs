using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class db2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VinNumber",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalElectricityUsed",
                table: "StatusReports");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerVehicleName",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmissionLevel",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "PossibleFuelTypes",
                table: "Vehicles",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalBatteryPackCapacity",
                table: "Vehicles",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalFuelTankCapacityGaseous",
                table: "Vehicles",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalFuelTankVolume",
                table: "Vehicles",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vin",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalEngineHours",
                table: "StatusReports",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "TotalElectricMotorHours",
                table: "StatusReports",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopTime",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<double>(
                name: "EngineTotalFuelUsed",
                table: "StatusReports",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<double>(
                name: "BatteryPack",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CatalystFuelLevel",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "StatusReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ElectricMotorSpeed",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EngineSpeed",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelConsumptionDuringCruiseActive",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelConsumptionDuringCruiseActiveGaseous",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelLevel1",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelLevel2",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelToEmpty",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "StatusReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GasToEmpty",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HrTotalVehicleDistance",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ignition",
                table: "StatusReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdatedBy",
                table: "StatusReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDateTime",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalElectricEnergyUsed",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalGaseousFuelUsed",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalToEmpty",
                table: "StatusReports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vin",
                table: "StatusReports",
                type: "text",
                nullable: true);

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
                name: "VehicleEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "text", nullable: true),
                    TotalEngineTime = table.Column<double>(type: "double precision", nullable: true),
                    AvgSpeed = table.Column<double>(type: "double precision", nullable: true),
                    AvgFuelConsumption = table.Column<double>(type: "double precision", nullable: true),
                    AvgElectricEnergyConsumption = table.Column<double>(type: "double precision", nullable: true),
                    TotalFuelConsumption = table.Column<double>(type: "double precision", nullable: true),
                    FuelConsumptionPerHour = table.Column<double>(type: "double precision", nullable: true),
                    Co2Emissions = table.Column<double>(type: "double precision", nullable: true),
                    Co2Saved = table.Column<double>(type: "double precision", nullable: true),
                    TotalDistance = table.Column<double>(type: "double precision", nullable: true),
                    TotalGasUsed = table.Column<double>(type: "double precision", nullable: true),
                    EngineRunningTime = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    EsgVehicleReportId = table.Column<Guid>(type: "uuid", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_VehicleEvaluations_EsgVehicleReportId",
                table: "VehicleEvaluations",
                column: "EsgVehicleReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleEvaluations");

            migrationBuilder.DropTable(
                name: "EsgVehicleReports");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CustomerVehicleName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EmissionLevel",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PossibleFuelTypes",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalBatteryPackCapacity",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalFuelTankCapacityGaseous",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalFuelTankVolume",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Vin",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BatteryPack",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "CatalystFuelLevel",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "ElectricMotorSpeed",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "EngineSpeed",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "FuelConsumptionDuringCruiseActive",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "FuelConsumptionDuringCruiseActiveGaseous",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "FuelLevel1",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "FuelLevel2",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "FuelToEmpty",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "GasToEmpty",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "HrTotalVehicleDistance",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "Ignition",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "ReceivedDateTime",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "TotalElectricEnergyUsed",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "TotalGaseousFuelUsed",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "TotalToEmpty",
                table: "StatusReports");

            migrationBuilder.DropColumn(
                name: "Vin",
                table: "StatusReports");

            migrationBuilder.AddColumn<string>(
                name: "VinNumber",
                table: "Vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "TotalEngineHours",
                table: "StatusReports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalElectricMotorHours",
                table: "StatusReports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StopTime",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "StatusReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "EngineTotalFuelUsed",
                table: "StatusReports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalElectricityUsed",
                table: "StatusReports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
