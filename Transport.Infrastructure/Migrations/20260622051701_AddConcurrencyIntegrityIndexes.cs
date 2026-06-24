using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrencyIntegrityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM Trips
                    WHERE [Status] = 'InProgress'
                    GROUP BY RouteAssignmentId
                    HAVING COUNT(*) > 1
                )
                BEGIN
                    THROW 51000, 'No se puede crear el indice unico de viajes activos porque existen multiples viajes InProgress para una misma asignacion.', 1;
                END
                """);

            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM RouteStops
                    GROUP BY RouteId, StopId
                    HAVING COUNT(*) > 1
                )
                BEGIN
                    THROW 51001, 'No se puede crear el indice unico de paradas porque existen paradas duplicadas en una misma ruta.', 1;
                END
                """);

            migrationBuilder.DropIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips",
                column: "RouteAssignmentId",
                unique: true,
                filter: "[Status] = 'InProgress'");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_StopId",
                table: "RouteStops",
                columns: new[] { "RouteId", "StopId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_RouteId_StopId",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_IsRead_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips",
                column: "RouteAssignmentId");
        }
    }
}
