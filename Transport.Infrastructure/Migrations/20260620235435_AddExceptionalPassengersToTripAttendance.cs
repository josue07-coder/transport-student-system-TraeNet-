using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExceptionalPassengersToTripAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExceptionReason",
                table: "TripStudentAttendances",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExpectedPassenger",
                table: "TripStudentAttendances",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAt",
                table: "TripStudentAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegisteredByUserId",
                table: "TripStudentAttendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripStudentAttendances_RegisteredByUserId",
                table: "TripStudentAttendances",
                column: "RegisteredByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripStudentAttendances_Users_RegisteredByUserId",
                table: "TripStudentAttendances",
                column: "RegisteredByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripStudentAttendances_Users_RegisteredByUserId",
                table: "TripStudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_TripStudentAttendances_RegisteredByUserId",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "ExceptionReason",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "IsExpectedPassenger",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "TripStudentAttendances");

            migrationBuilder.DropColumn(
                name: "RegisteredByUserId",
                table: "TripStudentAttendances");
        }
    }
}
