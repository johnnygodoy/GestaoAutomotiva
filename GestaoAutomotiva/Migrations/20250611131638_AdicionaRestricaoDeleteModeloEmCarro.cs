using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaRestricaoDeleteModeloEmCarro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carros_Modelos_ModeloId",
                table: "Carros");

            migrationBuilder.AddColumn<int>(
                name: "ModeloId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_ModeloId",
                table: "AcessoriosCarros",
                column: "ModeloId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Modelos_ModeloId",
                table: "AcessoriosCarros",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carros_Modelos_ModeloId",
                table: "Carros",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Modelos_ModeloId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_Carros_Modelos_ModeloId",
                table: "Carros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_ModeloId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "ModeloId",
                table: "AcessoriosCarros");

            migrationBuilder.AddForeignKey(
                name: "FK_Carros_Modelos_ModeloId",
                table: "Carros",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
