using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class IndtoListforwallets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WalletId",
                table: "CurrencyWallet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_WalletId",
                table: "CurrencyWallet",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletId",
                table: "CurrencyWallet",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletId",
                table: "CurrencyWallet");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyWallet_WalletId",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "CurrencyWallet");
        }
    }
}
