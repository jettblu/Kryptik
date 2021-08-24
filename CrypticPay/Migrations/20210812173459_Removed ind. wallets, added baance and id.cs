using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class Removedindwalletsaddedbaanceandid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_CurrencyWallet_BitcoinCashWalletId",
                table: "Wallet");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_CurrencyWallet_BitcoinWalletId",
                table: "Wallet");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_CurrencyWallet_EthereumWalletId",
                table: "Wallet");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_CurrencyWallet_LitecoinWalletId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_BitcoinCashWalletId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_BitcoinWalletId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_EthereumWalletId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_LitecoinWalletId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "BitcoinCashWalletId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "BitcoinWalletId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "EthereumWalletId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "LitecoinWalletId",
                table: "Wallet");

            migrationBuilder.AddColumn<double>(
                name: "AccountBalanceCrypto",
                table: "CurrencyWallet",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AccountBalanceFiat",
                table: "CurrencyWallet",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalanceCrypto",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "AccountBalanceFiat",
                table: "CurrencyWallet");

            migrationBuilder.AddColumn<string>(
                name: "BitcoinCashWalletId",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BitcoinWalletId",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EthereumWalletId",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LitecoinWalletId",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_BitcoinCashWalletId",
                table: "Wallet",
                column: "BitcoinCashWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_BitcoinWalletId",
                table: "Wallet",
                column: "BitcoinWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_EthereumWalletId",
                table: "Wallet",
                column: "EthereumWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_LitecoinWalletId",
                table: "Wallet",
                column: "LitecoinWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_CurrencyWallet_BitcoinCashWalletId",
                table: "Wallet",
                column: "BitcoinCashWalletId",
                principalTable: "CurrencyWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_CurrencyWallet_BitcoinWalletId",
                table: "Wallet",
                column: "BitcoinWalletId",
                principalTable: "CurrencyWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_CurrencyWallet_EthereumWalletId",
                table: "Wallet",
                column: "EthereumWalletId",
                principalTable: "CurrencyWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_CurrencyWallet_LitecoinWalletId",
                table: "Wallet",
                column: "LitecoinWalletId",
                principalTable: "CurrencyWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
