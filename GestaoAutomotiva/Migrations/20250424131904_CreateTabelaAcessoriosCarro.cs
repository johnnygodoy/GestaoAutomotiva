using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class CreateTabelaAcessoriosCarro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcessoriosCarroId",
                table: "Carros",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcessoriosCarros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Motor = table.Column<string>(type: "TEXT", nullable: false),
                    Cambio = table.Column<string>(type: "TEXT", nullable: false),
                    Suspensao = table.Column<string>(type: "TEXT", nullable: false),
                    RodasPneus = table.Column<string>(type: "TEXT", nullable: false),
                    Carroceria = table.Column<string>(type: "TEXT", nullable: false),
                    Capota = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcessoriosCarros", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carros_AcessoriosCarroId",
                table: "Carros",
                column: "AcessoriosCarroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                table: "Carros",
                column: "AcessoriosCarroId",
                principalTable: "AcessoriosCarros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                table: "Carros");

            migrationBuilder.DropTable(
                name: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_Carros_AcessoriosCarroId",
                table: "Carros");

            migrationBuilder.DropColumn(
                name: "AcessoriosCarroId",
                table: "Carros");
        }
    }
}
