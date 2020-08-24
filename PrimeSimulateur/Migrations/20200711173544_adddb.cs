using Microsoft.EntityFrameworkCore.Migrations;

namespace PrimeSimulateur.Migrations
{
    public partial class adddb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: false),
                    firstName = table.Column<string>(nullable: true),
                    lastName = table.Column<string>(nullable: true),
                    number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logements",
                columns: table => new
                {
                    LogementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    adresse = table.Column<string>(nullable: false),
                    Ville = table.Column<string>(nullable: true),
                    TypeEnergie = table.Column<string>(nullable: true),
                    surface = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logements", x => x.LogementId);
                    table.ForeignKey(
                        name: "FK_Logements_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Situations",
                columns: table => new
                {
                    SituationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombredepersonne = table.Column<int>(nullable: false),
                    Revenumenage = table.Column<float>(nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Situations", x => x.SituationId);
                    table.ForeignKey(
                        name: "FK_Situations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Travails",
                columns: table => new
                {
                    TravailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    surface = table.Column<float>(nullable: false),
                    LogementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travails", x => x.TravailId);
                    table.ForeignKey(
                        name: "FK_Travails_Logements_LogementId",
                        column: x => x.LogementId,
                        principalTable: "Logements",
                        principalColumn: "LogementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategorieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(nullable: true),
                    TravailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategorieId);
                    table.ForeignKey(
                        name: "FK_Categories_Travails_TravailId",
                        column: x => x.TravailId,
                        principalTable: "Travails",
                        principalColumn: "TravailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TravailId",
                table: "Categories",
                column: "TravailId");

            migrationBuilder.CreateIndex(
                name: "IX_Logements_ClientId",
                table: "Logements",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Situations_ClientId",
                table: "Situations",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Travails_LogementId",
                table: "Travails",
                column: "LogementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ProjectRole");

            migrationBuilder.DropTable(
                name: "Situations");

            migrationBuilder.DropTable(
                name: "Travails");

            migrationBuilder.DropTable(
                name: "Logements");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
