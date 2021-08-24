using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class Liscoinswallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "Mnemonic",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "XPub",
                table: "CurrencyWallet");

            migrationBuilder.AddColumn<string>(
                name: "Xpub",
                table: "Wallet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoinId",
                table: "CurrencyWallet",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CrypticPayCoins",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ApiTag = table.Column<string>(nullable: true),
                    Ticker = table.Column<string>(nullable: true),
                    ThumbnailPath = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    IsSupported = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrypticPayCoins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_CoinId",
                table: "CurrencyWallet",
                column: "CoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_CrypticPayCoins_CoinId",
                table: "CurrencyWallet",
                column: "CoinId",
                principalTable: "CrypticPayCoins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_CrypticPayCoins_CoinId",
                table: "CurrencyWallet");

            migrationBuilder.DropTable(
                name: "CrypticPayCoins");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyWallet_CoinId",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "Xpub",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "CoinId",
                table: "CurrencyWallet");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "CurrencyWallet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mnemonic",
                table: "CurrencyWallet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XPub",
                table: "CurrencyWallet",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
