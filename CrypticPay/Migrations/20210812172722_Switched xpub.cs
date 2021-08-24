using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class Switchedxpub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Xpub",
                table: "Wallet");

            migrationBuilder.AddColumn<string>(
                name: "Xpub",
                table: "CurrencyWallet",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Xpub",
                table: "CurrencyWallet");

            migrationBuilder.AddColumn<string>(
                name: "Xpub",
                table: "Wallet",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
