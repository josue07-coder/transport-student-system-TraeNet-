using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripStartRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DelayMinutes",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EarlyStartReason",
                table: "Trips",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PunctualityStatus",
                table: "Trips",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "OnTime");

            migrationBuilder.AddColumn<bool>(
                name: "StartedEarly",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelayMinutes",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EarlyStartReason",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PunctualityStatus",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartedEarly",
                table: "Trips");
        }
    }
}
