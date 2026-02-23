using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensorService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TalhaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoilMoisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Precipitation = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_TalhaoId_TimestampUtc",
                table: "SensorReadings",
                columns: new[] { "TalhaoId", "TimestampUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorReadings");
        }
    }
}
