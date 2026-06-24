using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trips",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Routes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_OperationDate",
                table: "Trips",
                column: "OperationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteAssignmentId_Status",
                table: "Trips",
                columns: new[] { "RouteAssignmentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips",
                column: "RouteAssignmentId",
                unique: true,
                filter: "[Status] = 'InProgress'");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ScheduledDepartureTime",
                table: "Trips",
                column: "ScheduledDepartureTime");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartTime",
                table: "Trips",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Status",
                table: "Trips",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Status_StartTime",
                table: "Trips",
                columns: new[] { "Status", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_IsActive",
                table: "Students",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentCode",
                table: "Students",
                column: "StudentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_SchoolId_Name",
                table: "Routes",
                columns: new[] { "SchoolId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Status",
                table: "Routes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Severity_CreatedAt",
                table: "Incidents",
                columns: new[] { "Severity", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Status_CreatedAt",
                table: "Incidents",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "Action", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_OperationDate",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_RouteAssignmentId_Status",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ScheduledDepartureTime",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_StartTime",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Status",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Status_StartTime",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Students_IsActive",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentCode",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Routes_SchoolId_Name",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_Status",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_Severity_CreatedAt",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_Status_CreatedAt",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Action_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RouteAssignmentId",
                table: "Trips",
                column: "RouteAssignmentId",
                unique: true,
                filter: "[Status] = 'InProgress'");
        }
    }
}
