using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class WalletInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WalletKryptikId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CurrencyWallet",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    XPub = table.Column<string>(nullable: true),
                    Mnemonic = table.Column<string>(nullable: true),
                    DepositAddress = table.Column<string>(nullable: true),
                    AccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyWallet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BitcoinWalletId = table.Column<string>(nullable: true),
                    EthereumWalletId = table.Column<string>(nullable: true),
                    BitcoinCashWalletId = table.Column<string>(nullable: true),
                    LitecoinWalletId = table.Column<string>(nullable: true),
                    Phrase = table.Column<byte[]>(nullable: true),
                    Decrypter = table.Column<byte[]>(nullable: true),
                    Iv = table.Column<byte[]>(nullable: true),
                    IsCustodial = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallet_CurrencyWallet_BitcoinCashWalletId",
                        column: x => x.BitcoinCashWalletId,
                        principalTable: "CurrencyWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wallet_CurrencyWallet_BitcoinWalletId",
                        column: x => x.BitcoinWalletId,
                        principalTable: "CurrencyWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wallet_CurrencyWallet_EthereumWalletId",
                        column: x => x.EthereumWalletId,
                        principalTable: "CurrencyWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wallet_CurrencyWallet_LitecoinWalletId",
                        column: x => x.LitecoinWalletId,
                        principalTable: "CurrencyWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WalletKryptikId",
                table: "AspNetUsers",
                column: "WalletKryptikId");

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
                name: "FK_AspNetUsers_Wallet_WalletKryptikId",
                table: "AspNetUsers",
                column: "WalletKryptikId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Wallet_WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "CurrencyWallet");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WalletKryptikId",
                table: "AspNetUsers");
        }
    }
}
