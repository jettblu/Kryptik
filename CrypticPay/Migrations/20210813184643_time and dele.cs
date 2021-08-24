using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class timeanddele : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Wallet_WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WalletKryptikId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "CrypticPayUserKey",
                table: "Wallet",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Wallet",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CurrencyWallet",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CrypticPayUserKey",
                table: "Wallet",
                column: "CrypticPayUserKey",
                unique: true,
                filter: "[CrypticPayUserKey] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserKey",
                table: "Wallet",
                column: "CrypticPayUserKey",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserKey",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_CrypticPayUserKey",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CurrencyWallet");

            migrationBuilder.AlterColumn<string>(
                name: "CrypticPayUserKey",
                table: "Wallet",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletKryptikId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

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
        }
    }
}
