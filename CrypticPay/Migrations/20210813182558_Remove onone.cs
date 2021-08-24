using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class Removeonone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_AspNetUsers_CrypticPayUserKey",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_CrypticPayUserKey",
                table: "Wallet");

            migrationBuilder.AlterColumn<string>(
                name: "CrypticPayUserKey",
                table: "Wallet",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletKryptikId",
                table: "AspNetUsers",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

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
    }
}
