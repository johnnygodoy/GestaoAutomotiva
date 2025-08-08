using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class SantoAntonio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_RodasPneus_RodasPneusId",
                table: "AcessoriosCarros");

            migrationBuilder.DropTable(
                name: "RodasPneus");

            migrationBuilder.RenameColumn(
                name: "RodasPneusId",
                table: "AcessoriosCarros",
                newName: "SantoAntonioId");

            migrationBuilder.RenameIndex(
                name: "IX_AcessoriosCarros_RodasPneusId",
                table: "AcessoriosCarros",
                newName: "IX_AcessoriosCarros_SantoAntonioId");

            migrationBuilder.AddColumn<int>(
                name: "PneuId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RodaId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Pneus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pneus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pneus_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rodas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rodas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rodas_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SantoAntonios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SantoAntonios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SantoAntonios_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_PneuId",
                table: "AcessoriosCarros",
                column: "PneuId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessoriosCarros_RodaId",
                table: "AcessoriosCarros",
                column: "RodaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pneus_ModeloId",
                table: "Pneus",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Rodas_ModeloId",
                table: "Rodas",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_SantoAntonios_ModeloId",
                table: "SantoAntonios",
                column: "ModeloId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Pneus_PneuId",
                table: "AcessoriosCarros",
                column: "PneuId",
                principalTable: "Pneus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Rodas_RodaId",
                table: "AcessoriosCarros",
                column: "RodaId",
                principalTable: "Rodas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_SantoAntonios_SantoAntonioId",
                table: "AcessoriosCarros",
                column: "SantoAntonioId",
                principalTable: "SantoAntonios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Pneus_PneuId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Rodas_RodaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_SantoAntonios_SantoAntonioId",
                table: "AcessoriosCarros");

            migrationBuilder.DropTable(
                name: "Pneus");

            migrationBuilder.DropTable(
                name: "Rodas");

            migrationBuilder.DropTable(
                name: "SantoAntonios");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_PneuId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_RodaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "PneuId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "RodaId",
                table: "AcessoriosCarros");

            migrationBuilder.RenameColumn(
                name: "SantoAntonioId",
                table: "AcessoriosCarros",
                newName: "RodasPneusId");

            migrationBuilder.RenameIndex(
                name: "IX_AcessoriosCarros_SantoAntonioId",
                table: "AcessoriosCarros",
                newName: "IX_AcessoriosCarros_RodasPneusId");

            migrationBuilder.CreateTable(
                name: "RodasPneus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModeloId = table.Column<int>(type: "INTEGER", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodasPneus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RodasPneus_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RodasPneus_ModeloId",
                table: "RodasPneus",
                column: "ModeloId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_RodasPneus_RodasPneusId",
                table: "AcessoriosCarros",
                column: "RodasPneusId",
                principalTable: "RodasPneus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
