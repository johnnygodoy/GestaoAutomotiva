using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjusteModelsOrdemServico4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Carros_CarroId",
                table: "OrdemServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Funcionarios_FuncionarioId",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_CarroId",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_FuncionarioId",
                table: "OrdemServicos");

            migrationBuilder.AlterColumn<int>(
                name: "FuncionarioId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CarroId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "OrdemServicos");

            migrationBuilder.AlterColumn<int>(
                name: "FuncionarioId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarroId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_CarroId",
                table: "OrdemServicos",
                column: "CarroId");

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
                name: "FK_OrdemServicos_Funcionarios_FuncionarioId",
                table: "OrdemServicos",
                column: "FuncionarioId",
                principalTable: "Funcionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
