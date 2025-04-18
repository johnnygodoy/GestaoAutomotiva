using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjusteModelsOrdemServico3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Etapas_EtapaId",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_EtapaId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "EtapaId",
                table: "OrdemServicos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EtapaId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_EtapaId",
                table: "OrdemServicos",
                column: "EtapaId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Etapas_EtapaId",
                table: "OrdemServicos",
                column: "EtapaId",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
