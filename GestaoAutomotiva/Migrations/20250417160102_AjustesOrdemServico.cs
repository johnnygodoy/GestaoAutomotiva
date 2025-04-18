using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjustesOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarroId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EtapaId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuncionarioId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_CarroId",
                table: "OrdemServicos",
                column: "CarroId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_EtapaId",
                table: "OrdemServicos",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_FuncionarioId",
                table: "OrdemServicos",
                column: "FuncionarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Carros_CarroId",
                table: "OrdemServicos",
                column: "CarroId",
                principalTable: "Carros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Etapas_EtapaId",
                table: "OrdemServicos",
                column: "EtapaId",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Funcionarios_FuncionarioId",
                table: "OrdemServicos",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Carros_CarroId",
                table: "OrdemServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Etapas_EtapaId",
                table: "OrdemServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Funcionarios_FuncionarioId",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_CarroId",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_EtapaId",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_FuncionarioId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "CarroId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "EtapaId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "FuncionarioId",
                table: "OrdemServicos");
        }
    }
}
