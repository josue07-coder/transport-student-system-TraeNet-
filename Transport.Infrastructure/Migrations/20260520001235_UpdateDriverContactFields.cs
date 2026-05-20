using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDriverContactFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Drivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Cedula");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Drivers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Drivers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE Drivers SET DocumentNumber = CONCAT('PENDING-', CONVERT(nvarchar(36), Id)) WHERE DocumentNumber = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_SchoolId",
                table: "Routes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_DocumentNumber",
                table: "Drivers",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Schools_SchoolId",
                table: "Routes",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Schools_SchoolId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_SchoolId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_DocumentNumber",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Drivers");
        }
    }
}
