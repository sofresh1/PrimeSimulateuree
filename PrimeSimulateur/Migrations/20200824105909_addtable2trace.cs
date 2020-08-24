using Microsoft.EntityFrameworkCore.Migrations;

namespace PrimeSimulateur.Migrations
{
    public partial class addtable2trace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Situations_ClientId",
                table: "Situations");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "trace",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "trace",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Situations_ClientId",
                table: "Situations",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Situations_ClientId",
                table: "Situations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "trace");

            migrationBuilder.DropColumn(
                name: "email",
                table: "trace");

            migrationBuilder.CreateIndex(
                name: "IX_Situations_ClientId",
                table: "Situations",
                column: "ClientId",
                unique: true);
        }
    }
}
