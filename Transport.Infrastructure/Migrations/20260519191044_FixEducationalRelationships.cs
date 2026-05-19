using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEducationalRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guardians_Sectors_SectorId1",
                table: "Guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_SchoolDistricts_SchoolDistrictId1",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_SchoolDistrictId1",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_SectorId1",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "SchoolDistrictId1",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "SectorId1",
                table: "Guardians");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolId",
                table: "Students",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Schools_SchoolId",
                table: "Students",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Schools_SchoolId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolId",
                table: "Students");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolDistrictId1",
                table: "Sectors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectorId1",
                table: "Guardians",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_SchoolDistrictId1",
                table: "Sectors",
                column: "SchoolDistrictId1");

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_SectorId1",
                table: "Guardians",
                column: "SectorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Guardians_Sectors_SectorId1",
                table: "Guardians",
                column: "SectorId1",
                principalTable: "Sectors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_SchoolDistricts_SchoolDistrictId1",
                table: "Sectors",
                column: "SchoolDistrictId1",
                principalTable: "SchoolDistricts",
                principalColumn: "Id");
        }
    }
}
