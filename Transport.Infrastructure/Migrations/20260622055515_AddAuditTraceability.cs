using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTraceability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "AuditLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestPath",
                table: "AuditLogs",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TraceId",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CorrelationId",
                table: "AuditLogs",
                column: "CorrelationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CorrelationId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "RequestPath",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "TraceId",
                table: "AuditLogs");
        }
    }
}
