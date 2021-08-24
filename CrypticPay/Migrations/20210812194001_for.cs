using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class @for : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserForeignKey",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_CrypticPayUserForeignKey",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "CrypticPayUserForeignKey",
                table: "Wallet");

            migrationBuilder.AddColumn<string>(
                name: "CrypticPayUserKey",
                table: "Wallet",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
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
                name: "CrypticPayUserKey",
                table: "Wallet");

            migrationBuilder.AddColumn<string>(
                name: "CrypticPayUserForeignKey",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CrypticPayUserForeignKey",
                table: "Wallet",
                column: "CrypticPayUserForeignKey",
                unique: true,
                filter: "[CrypticPayUserForeignKey] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserForeignKey",
                table: "Wallet",
                column: "CrypticPayUserForeignKey",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
