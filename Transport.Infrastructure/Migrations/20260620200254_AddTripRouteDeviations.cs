using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripRouteDeviations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripRouteDeviations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReasonType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripRouteDeviations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripRouteDeviations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripRouteDeviations_Users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripRouteDeviations_ReportedAt",
                table: "TripRouteDeviations",
                column: "ReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripRouteDeviations_ReportedByUserId",
                table: "TripRouteDeviations",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TripRouteDeviations_TripId",
                table: "TripRouteDeviations",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripRouteDeviations");
        }
    }
}
