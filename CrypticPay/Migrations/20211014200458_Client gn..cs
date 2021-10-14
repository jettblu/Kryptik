using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class Clientgn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustodial",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "XpubMaster",
                table: "BlockchainAddress");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnChain",
                table: "Wallet",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SeedShare",
                table: "Wallet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Xpub",
                table: "Wallet",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnChain",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "SeedShare",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "Xpub",
                table: "Wallet");

            migrationBuilder.AddColumn<bool>(
                name: "IsCustodial",
                table: "Wallet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "XpubMaster",
                table: "BlockchainAddress",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
