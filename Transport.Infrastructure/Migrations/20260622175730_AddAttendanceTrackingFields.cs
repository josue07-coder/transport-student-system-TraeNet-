using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AbsenceNotifiedAt",
                table: "TripStudentAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttendanceSource",
                table: "TripStudentAttendances",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.AddColumn<DateTime>(
                name: "BoardedNotificationSentAt",
                table: "TripStudentAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MarkedAt",
                table: "TripStudentAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MarkedByUserId",
                table: "TripStudentAttendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripStudentAttendances_MarkedByUserId",
                table: "TripStudentAttendances",
                column: "MarkedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TripStudentAttendances_StudentId_CreatedAt",
                table: "TripStudentAttendances",
                columns: new[] { "StudentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TripStudentAttendances_TripId_Status",
                table: "TripStudentAttendances",
                columns: new[] { "TripId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_TripStudentAttendances_Users_MarkedByUserId",
                table: "TripStudentAttendances",
                column: "MarkedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripStudentAttendances_Users_MarkedByUserId",
                table: "TripStudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_TripStudentAttendances_MarkedByUserId",
                table: "TripStudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_TripStudentAttendances_StudentId_CreatedAt",
                table: "TripStudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_TripStudentAttendances_TripId_Status",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "AbsenceNotifiedAt",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "AttendanceSource",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "BoardedNotificationSentAt",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "MarkedAt",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "MarkedByUserId",
                table: "TripStudentAttendances");
        }
    }
}
