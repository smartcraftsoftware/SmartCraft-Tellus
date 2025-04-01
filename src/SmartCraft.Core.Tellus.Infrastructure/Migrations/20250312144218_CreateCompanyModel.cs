using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateCompanyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VolvoCredentials = table.Column<string>(type: "text", nullable: true),
                    ScaniaClientId = table.Column<string>(type: "text", nullable: true),
                    ScaniaSecretKey = table.Column<string>(type: "text", nullable: true),
                    ManToken = table.Column<string>(type: "text", nullable: true),
                    DaimlerToken = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Id_TenantId",
                table: "Companies",
                columns: new[] { "Id", "TenantId" },
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO ""Companies"" (
                    ""Id"", ""TenantId"", ""VolvoCredentials"", ""ScaniaClientId"", ""ScaniaSecretKey"", 
                    ""ManToken"", ""DaimlerToken"", ""CreatedAt"", ""LastUpdated"", ""CreatedBy"", ""LastUpdatedBy"")
                SELECT 
                    gen_random_uuid(), ""Id"", ""VolvoCredentials"", ""ScaniaClientId"", ""ScaniaSecretKey"", 
                    ""ManToken"", ""DaimlerToken"", NOW(), NOW(), ""Id"", ""Id""
                FROM ""tenants"";
            ");

            migrationBuilder.DropTable(
                name: "Tenants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Tenants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                DaimlerToken = table.Column<string>(type: "text", nullable: true),
                LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                ManToken = table.Column<string>(type: "text", nullable: true),
                ScaniaClientId = table.Column<string>(type: "text", nullable: true),
                ScaniaSecretKey = table.Column<string>(type: "text", nullable: true),
                VolvoCredentials = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

            migrationBuilder.Sql(@"
            INSERT INTO ""Tenants"" (
                ""Id"", ""VolvoCredentials"", ""ScaniaClientId"", ""ScaniaSecretKey"", 
                ""ManToken"", ""DaimlerToken"", ""CreatedAt"", ""LastUpdated"", ""CreatedBy"", ""LastUpdatedBy"")
            SELECT 
                ""TenantId"", ""VolvoCredentials"", ""ScaniaClientId"", ""ScaniaSecretKey"", 
                ""ManToken"", ""DaimlerToken"", ""CreatedAt"", ""LastUpdated"", ""CreatedBy"", ""LastUpdatedBy""
            FROM ""Companies"";
        ");
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
