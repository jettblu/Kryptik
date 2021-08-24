using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class Cacader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Wallet_WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_CrypticPayCoins_CoinId",
                table: "CurrencyWallet");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletId",
                table: "CurrencyWallet");

            migrationBuilder.DropTable(
                name: "CrypticPayCoins");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyWallet_CoinId",
                table: "CurrencyWallet");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyWallet_WalletId",
                table: "CurrencyWallet");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CrypticPayUserForeignKey",
                table: "Wallet",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoinId",
                table: "CurrencyWallet",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletKryptikId",
                table: "CurrencyWallet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CrypticPayUserForeignKey",
                table: "Wallet",
                column: "CrypticPayUserForeignKey",
                unique: true,
                filter: "[CrypticPayUserForeignKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_WalletKryptikId",
                table: "CurrencyWallet",
                column: "WalletKryptikId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletKryptikId",
                table: "CurrencyWallet",
                column: "WalletKryptikId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserForeignKey",
                table: "Wallet",
                column: "CrypticPayUserForeignKey",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletKryptikId",
                table: "CurrencyWallet");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserForeignKey",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_CrypticPayUserForeignKey",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyWallet_WalletKryptikId",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "CrypticPayUserForeignKey",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "WalletKryptikId",
                table: "CurrencyWallet");

            migrationBuilder.AlterColumn<string>(
                name: "CoinId",
                table: "CurrencyWallet",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletId",
                table: "CurrencyWallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletKryptikId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CrypticPayCoins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApiTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSupported = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrypticPayCoins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_CoinId",
                table: "CurrencyWallet",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_WalletId",
                table: "CurrencyWallet",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WalletKryptikId",
                table: "AspNetUsers",
                column: "WalletKryptikId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Wallet_WalletKryptikId",
                table: "AspNetUsers",
                column: "WalletKryptikId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_CrypticPayCoins_CoinId",
                table: "CurrencyWallet",
                column: "CoinId",
                principalTable: "CrypticPayCoins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_Wallet_WalletId",
                table: "CurrencyWallet",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
