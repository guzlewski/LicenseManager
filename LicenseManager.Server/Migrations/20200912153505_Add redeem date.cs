using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LicenseManager.Server.Migrations
{
    public partial class Addredeemdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RedeemDate",
                schema: "LicenseManager",
                table: "Licenses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedeemDate",
                schema: "LicenseManager",
                table: "Licenses");
        }
    }
}
