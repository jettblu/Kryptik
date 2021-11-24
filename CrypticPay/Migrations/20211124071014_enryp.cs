using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations
{
    public partial class enryp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyPath",
                table: "Chats",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageFrom",
                table: "Chats",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageTo",
                table: "Chats",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyPath",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "MessageFrom",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "MessageTo",
                table: "Chats");
        }
    }
}
