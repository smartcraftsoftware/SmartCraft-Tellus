using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations;

/// <inheritdoc />
public partial class updatedsomefields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "EngineRunningTime",
            table: "VehicleEvaluations",
            type: "text",
            nullable: true,
            oldClrType: typeof(DateOnly),
            oldType: "date",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateOnly>(
            name: "EngineRunningTime",
            table: "VehicleEvaluations",
            type: "date",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);
    }
}
