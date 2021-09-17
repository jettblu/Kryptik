using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class switchxpubloc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Xpub",
                table: "CurrencyWallet");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "BlockchainAddress",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "BlockchainAddress",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "XpubMaster",
                table: "BlockchainAddress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "BlockchainAddress");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "BlockchainAddress");

            migrationBuilder.DropColumn(
                name: "XpubMaster",
                table: "BlockchainAddress");

            migrationBuilder.AddColumn<string>(
                name: "Xpub",
                table: "CurrencyWallet",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
