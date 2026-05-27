using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Speed = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Heading = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleLocations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLocations_Users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLocations_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLocations_RecordedAt",
                table: "VehicleLocations",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLocations_ReportedByUserId",
                table: "VehicleLocations",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLocations_TripId",
                table: "VehicleLocations",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLocations_TripId_RecordedAt",
                table: "VehicleLocations",
                columns: new[] { "TripId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLocations_VehicleId",
                table: "VehicleLocations",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleLocations");
        }
    }
}
