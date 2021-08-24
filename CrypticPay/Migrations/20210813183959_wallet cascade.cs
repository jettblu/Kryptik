using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class walletcascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletKryptikId",
                table: "CurrencyWallet");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletKryptikId",
                table: "CurrencyWallet",
                column: "WalletKryptikId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletKryptikId",
                table: "CurrencyWallet");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletKryptikId",
                table: "CurrencyWallet",
                column: "WalletKryptikId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
