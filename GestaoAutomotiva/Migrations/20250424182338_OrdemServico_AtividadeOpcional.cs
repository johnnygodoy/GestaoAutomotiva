using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class OrdemServico_AtividadeOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Atividades_AtividadeId",
                table: "OrdemServicos");

            migrationBuilder.AlterColumn<int>(
                name: "AtividadeId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Atividades_AtividadeId",
                table: "OrdemServicos",
                column: "AtividadeId",
                principalTable: "Atividades",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Atividades_AtividadeId",
                table: "OrdemServicos");

            migrationBuilder.AlterColumn<int>(
                name: "AtividadeId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Atividades_AtividadeId",
                table: "OrdemServicos",
                column: "AtividadeId",
                principalTable: "Atividades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
