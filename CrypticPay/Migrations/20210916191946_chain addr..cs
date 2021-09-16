using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class chainaddr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressKey",
                table: "CurrencyWallet",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlockchainAddress",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainAddress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_AddressKey",
                table: "CurrencyWallet",
                column: "AddressKey",
                unique: true,
                filter: "[AddressKey] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyWallet_BlockchainAddress_AddressKey",
                table: "CurrencyWallet",
                column: "AddressKey",
                principalTable: "BlockchainAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyWallet_BlockchainAddress_AddressKey",
                table: "CurrencyWallet");

            migrationBuilder.DropTable(
                name: "BlockchainAddress");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyWallet_AddressKey",
                table: "CurrencyWallet");

            migrationBuilder.DropColumn(
                name: "AddressKey",
                table: "CurrencyWallet");
        }
    }
}
