using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class txadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    AmountCrypto = table.Column<decimal>(nullable: false),
                    AmountFiat = table.Column<decimal>(nullable: false),
                    CoinId = table.Column<string>(nullable: true),
                    SenderId = table.Column<string>(nullable: true),
                    RecieverId = table.Column<string>(nullable: true),
                    OutsideAddressTo = table.Column<string>(nullable: true),
                    StatusCurrent = table.Column<int>(nullable: false),
                    BroadcastType = table.Column<int>(nullable: false),
                    PrivacyLevel = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
