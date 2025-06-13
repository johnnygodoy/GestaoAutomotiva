using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjustesNovosCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EscapamentoId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PainelId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Escapamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escapamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Escapamentos_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Paineis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paineis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paineis_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_EscapamentoId",
                table: "AcessoriosCarros",
                column: "EscapamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_PainelId",
                table: "AcessoriosCarros",
                column: "PainelId");

            migrationBuilder.CreateIndex(
                name: "IX_Escapamentos_ModeloId",
                table: "Escapamentos",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Paineis_ModeloId",
                table: "Paineis",
                column: "ModeloId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Escapamentos_EscapamentoId",
                table: "AcessoriosCarros",
                column: "EscapamentoId",
                principalTable: "Escapamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Escapamentos_PainelId",
                table: "AcessoriosCarros",
                column: "PainelId",
                principalTable: "Escapamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Escapamentos_EscapamentoId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Escapamentos_PainelId",
                table: "AcessoriosCarros");

            migrationBuilder.DropTable(
                name: "Escapamentos");

            migrationBuilder.DropTable(
                name: "Paineis");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_EscapamentoId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_PainelId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "EscapamentoId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "PainelId",
                table: "AcessoriosCarros");
        }
    }
}
