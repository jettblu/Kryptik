using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations.CrypticPayCoin
{
    public partial class InitialAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ApiTag = table.Column<string>(nullable: true),
                    Ticker = table.Column<string>(nullable: true),
                    ThumbnailPath = table.Column<string>(nullable: true),
                    IsSupported = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coins");
        }
    }
}
