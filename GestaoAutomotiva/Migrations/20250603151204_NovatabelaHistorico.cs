using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class NovatabelaHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtividadeHistoricos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AtividadeId = table.Column<int>(type: "INTEGER", nullable: true),
                    FuncionarioNome = table.Column<string>(type: "TEXT", nullable: false),
                    ServicoDescricao = table.Column<string>(type: "TEXT", nullable: false),
                    CarroId = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloNome = table.Column<string>(type: "TEXT", nullable: false),
                    Cliente = table.Column<string>(type: "TEXT", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataPrevista = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    EtapaAtual = table.Column<string>(type: "TEXT", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Acao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadeHistoricos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtividadeHistoricos");
        }
    }
}
