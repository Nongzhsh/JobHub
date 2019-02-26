using Microsoft.EntityFrameworkCore.Migrations;

namespace Nongzhsh.JobHub.Migrations
{
    public partial class update_0140 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationName",
                table: "AbpAuditLogs",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AbpAuditLogs",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "AbpAuditLogs",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationName",
                table: "AbpAuditLogs");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AbpAuditLogs");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "AbpAuditLogs");
        }
    }
}
