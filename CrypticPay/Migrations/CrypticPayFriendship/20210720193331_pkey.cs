using Microsoft.EntityFrameworkCore.Migrations;

namespace CrypticPay.Migrations.CrypticPayFriendship
{
    public partial class pkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "friendID",
                table: "Friends");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "friendID",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
