using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations.CrypticPayFriendship
{
    public partial class InitFriendz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    friendID = table.Column<string>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    FriendFrom = table.Column<string>(nullable: true),
                    FriendTo = table.Column<string>(nullable: true),
                    RequestTime = table.Column<DateTime>(nullable: false),
                    BecameFriendsTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");
        }
    }
}
