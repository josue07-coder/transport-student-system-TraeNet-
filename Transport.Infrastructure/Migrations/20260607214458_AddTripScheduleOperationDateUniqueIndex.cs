using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripScheduleOperationDateUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trips_TripScheduleId_OperationDate",
                table: "Trips");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripScheduleId_OperationDate",
                table: "Trips",
                columns: new[] { "TripScheduleId", "OperationDate" },
                unique: true,
                filter: "[TripScheduleId] IS NOT NULL AND [OperationDate] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trips_TripScheduleId_OperationDate",
                table: "Trips");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripScheduleId_OperationDate",
                table: "Trips",
                columns: new[] { "TripScheduleId", "OperationDate" });
        }
    }
}
