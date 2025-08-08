using System;
using GestaoAutomotiva.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    CPF = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Etapas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etapas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funcionarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Especialidade = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modelos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    EstimativaDias = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Senha = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cambios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cambios_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Capotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Capotas_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carrocerias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrocerias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carrocerias_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Motors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    PlacaVeiculoDoador = table.Column<string>(type: "TEXT", nullable: false),
                    NumeroMotor = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Motors_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Motors_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RodasPneus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodasPneus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RodasPneus_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suspensaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspensaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suspensaos_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcessoriosCarros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MotorId = table.Column<int>(type: "INTEGER", nullable: false),
                    CambioId = table.Column<int>(type: "INTEGER", nullable: false),
                    SuspensaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    RodasPneusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarroceriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CapotaId = table.Column<int>(type: "INTEGER", nullable: false),                 
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcessoriosCarros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcessoriosCarros_Cambios_CambioId",
                        column: x => x.CambioId,
                        principalTable: "Cambios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcessoriosCarros_Capotas_CapotaId",
                        column: x => x.CapotaId,
                        principalTable: "Capotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcessoriosCarros_Carrocerias_CarroceriaId",
                        column: x => x.CarroceriaId,
                        principalTable: "Carrocerias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcessoriosCarros_Motors_MotorId",
                        column: x => x.MotorId,
                        principalTable: "Motors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcessoriosCarros_RodasPneus_RodasPneusId",
                        column: x => x.RodasPneusId,
                        principalTable: "RodasPneus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcessoriosCarros_Suspensaos_SuspensaoId",
                        column: x => x.SuspensaoId,
                        principalTable: "Suspensaos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCarro = table.Column<string>(type: "TEXT", nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", nullable: false),
                    Cor = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcessoriosCarroId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                        column: x => x.AcessoriosCarroId,
                        principalTable: "AcessoriosCarros",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Carros_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FuncionarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimativaDias = table.Column<int>(type: "INTEGER", nullable: false),
                    DataPrevista = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    EtapaId = table.Column<int>(type: "INTEGER", nullable: true),
                    CarroId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cor = table.Column<string>(type: "TEXT", nullable: false),
                    EtapaId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atividades_Carros_CarroId",
                        column: x => x.CarroId,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atividades_Etapas_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Atividades_Etapas_EtapaId1",
                        column: x => x.EtapaId1,
                        principalTable: "Etapas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Atividades_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atividades_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdemServicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AtividadeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Prioridade = table.Column<string>(type: "TEXT", nullable: false),
                    EtapaAtual = table.Column<string>(type: "TEXT", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    FuncionarioId = table.Column<int>(type: "INTEGER", nullable: true),
                    CarroId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    Almoxarifado = table.Column<string>(type: "TEXT", nullable: true),
                    Inspetor = table.Column<string>(type: "TEXT", nullable: true),
                    Tarefas = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdemServicos_Atividades_AtividadeId",
                        column: x => x.AtividadeId,
                        principalTable: "Atividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Modelos",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "FURLAN GT40" },
                    { 2, "FURLAN COBRA" },
                    { 3, "FURLAN SSK1929" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nome", "Senha", "Tipo" },
                values: new object[] { 1, "admin@gestao.com", "Administrador", "admin123", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_CambioId",
                table: "AcessoriosCarros",
                column: "CambioId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_CapotaId",
                table: "AcessoriosCarros",
                column: "CapotaId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_CarroceriaId",
                table: "AcessoriosCarros",
                column: "CarroceriaId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_MotorId",
                table: "AcessoriosCarros",
                column: "MotorId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_RodasPneusId",
                table: "AcessoriosCarros",
                column: "RodasPneusId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_SuspensaoId",
                table: "AcessoriosCarros",
                column: "SuspensaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_CarroId",
                table: "Atividades",
                column: "CarroId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_EtapaId",
                table: "Atividades",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_EtapaId1",
                table: "Atividades",
                column: "EtapaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_FuncionarioId",
                table: "Atividades",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_ServicoId",
                table: "Atividades",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cambios_ModeloId",
                table: "Cambios",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Capotas_ModeloId",
                table: "Capotas",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Carrocerias_ModeloId",
                table: "Carrocerias",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Carros_AcessoriosCarroId",
                table: "Carros",
                column: "AcessoriosCarroId");

            migrationBuilder.CreateIndex(
                name: "IX_Carros_ClienteId",
                table: "Carros",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Motors_ClienteId",
                table: "Motors",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Motors_ModeloId",
                table: "Motors",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_AtividadeId",
                table: "OrdemServicos",
                column: "AtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_RodasPneus_ModeloId",
                table: "RodasPneus",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Suspensaos_ModeloId",
                table: "Suspensaos",
                column: "ModeloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdemServicos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Atividades");

            migrationBuilder.DropTable(
                name: "Carros");

            migrationBuilder.DropTable(
                name: "Etapas");

            migrationBuilder.DropTable(
                name: "Funcionarios");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "AcessoriosCarros");

            migrationBuilder.DropTable(
                name: "Cambios");

            migrationBuilder.DropTable(
                name: "Capotas");

            migrationBuilder.DropTable(
                name: "Carrocerias");

            migrationBuilder.DropTable(
                name: "Motors");

            migrationBuilder.DropTable(
                name: "RodasPneus");

            migrationBuilder.DropTable(
                name: "Suspensaos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Modelos");
        }
    }
}
