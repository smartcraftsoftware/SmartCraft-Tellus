using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VinNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StopTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalEngineHours = table.Column<double>(type: "double precision", nullable: false),
                    TotalElectricMotorHours = table.Column<double>(type: "double precision", nullable: false),
                    EngineTotalFuelUsed = table.Column<double>(type: "double precision", nullable: false),
                    TotalElectricityUsed = table.Column<double>(type: "double precision", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusReports");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
