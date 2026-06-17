using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripLifecycleStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Trips",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonOperationNotes",
                table: "Trips",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonOperationReason",
                table: "Trips",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE [Trips] SET [Status] = 'Scheduled' WHERE [Status] = 'Pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Trips] SET [Status] = 'Pending' WHERE [Status] = 'Scheduled'");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "NonOperationNotes",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "NonOperationReason",
                table: "Trips");
        }
    }
}
