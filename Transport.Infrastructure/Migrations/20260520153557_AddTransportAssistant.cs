using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportAssistant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransportAssistantId",
                table: "RouteAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransportAssistants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportAssistants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteAssignments_TransportAssistantId",
                table: "RouteAssignments",
                column: "TransportAssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportAssistants_DocumentNumber",
                table: "TransportAssistants",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteAssignments_TransportAssistants_TransportAssistantId",
                table: "RouteAssignments",
                column: "TransportAssistantId",
                principalTable: "TransportAssistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteAssignments_TransportAssistants_TransportAssistantId",
                table: "RouteAssignments");

            migrationBuilder.DropTable(
                name: "TransportAssistants");

            migrationBuilder.DropIndex(
                name: "IX_RouteAssignments_TransportAssistantId",
                table: "RouteAssignments");

            migrationBuilder.DropColumn(
                name: "TransportAssistantId",
                table: "RouteAssignments");
        }
    }
}
