using Microsoft.EntityFrameworkCore.Migrations;

namespace PrimeSimulateur.Migrations
{
    public partial class addtabletracetoMyDB11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trace",
                columns: table => new
                {
                    traceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Nom = table.Column<string>(nullable: true),
                    Surface = table.Column<float>(nullable: false),
                    prime = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trace", x => x.traceId);
                    table.ForeignKey(
                        name: "FK_trace_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trace_ClientId",
                table: "trace",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trace");
        }
    }
}
