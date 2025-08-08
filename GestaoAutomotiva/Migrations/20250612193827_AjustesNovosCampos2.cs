using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjustesNovosCampos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Escapamentos_PainelId",
                table: "AcessoriosCarros");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Paineis_PainelId",
                table: "AcessoriosCarros",
                column: "PainelId",
                principalTable: "Paineis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Paineis_PainelId",
                table: "AcessoriosCarros");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Escapamentos_PainelId",
                table: "AcessoriosCarros",
                column: "PainelId",
                principalTable: "Escapamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
