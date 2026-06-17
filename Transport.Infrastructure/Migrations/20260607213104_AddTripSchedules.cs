using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Direction",
                table: "Trips",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "OperationDate",
                table: "Trips",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledArrivalTime",
                table: "Trips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDepartureTime",
                table: "Trips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TripScheduleId",
                table: "Trips",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TripSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DepartureTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ArrivalTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    Monday = table.Column<bool>(type: "bit", nullable: false),
                    Tuesday = table.Column<bool>(type: "bit", nullable: false),
                    Wednesday = table.Column<bool>(type: "bit", nullable: false),
                    Thursday = table.Column<bool>(type: "bit", nullable: false),
                    Friday = table.Column<bool>(type: "bit", nullable: false),
                    Saturday = table.Column<bool>(type: "bit", nullable: false),
                    Sunday = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripSchedules_RouteAssignments_RouteAssignmentId",
                        column: x => x.RouteAssignmentId,
                        principalTable: "RouteAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripScheduleId",
                table: "Trips",
                column: "TripScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripScheduleId_OperationDate",
                table: "Trips",
                columns: new[] { "TripScheduleId", "OperationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TripSchedules_RouteAssignmentId",
                table: "TripSchedules",
                column: "RouteAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TripSchedules_RouteAssignmentId_Direction_ValidFrom",
                table: "TripSchedules",
                columns: new[] { "RouteAssignmentId", "Direction", "ValidFrom" });

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_TripSchedules_TripScheduleId",
                table: "Trips",
                column: "TripScheduleId",
                principalTable: "TripSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_TripSchedules_TripScheduleId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "TripSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Trips_TripScheduleId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_TripScheduleId_OperationDate",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Direction",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "OperationDate",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ScheduledArrivalTime",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ScheduledDepartureTime",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "TripScheduleId",
                table: "Trips");
        }
    }
}
