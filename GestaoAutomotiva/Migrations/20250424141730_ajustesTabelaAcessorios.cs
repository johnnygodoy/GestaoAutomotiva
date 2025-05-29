using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class ajustesTabelaAcessorios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cambio",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "Capota",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "Carroceria",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "Motor",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "RodasPneus",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "Suspensao",
                table: "AcessoriosCarros");

            migrationBuilder.AddColumn<int>(
                name: "CambioId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CapotaId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CarroceriaId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MotorId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RodasPneusId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuspensaoId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cambios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Capotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capotas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carrocerias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrocerias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RodasPneus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodasPneus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suspensaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspensaos", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Cambios_CambioId",
                table: "AcessoriosCarros",
                column: "CambioId",
                principalTable: "Cambios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Capotas_CapotaId",
                table: "AcessoriosCarros",
                column: "CapotaId",
                principalTable: "Capotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Carrocerias_CarroceriaId",
                table: "AcessoriosCarros",
                column: "CarroceriaId",
                principalTable: "Carrocerias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Motors_MotorId",
                table: "AcessoriosCarros",
                column: "MotorId",
                principalTable: "Motors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_RodasPneus_RodasPneusId",
                table: "AcessoriosCarros",
                column: "RodasPneusId",
                principalTable: "RodasPneus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Suspensaos_SuspensaoId",
                table: "AcessoriosCarros",
                column: "SuspensaoId",
                principalTable: "Suspensaos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Cambios_CambioId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Capotas_CapotaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Carrocerias_CarroceriaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Motors_MotorId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_RodasPneus_RodasPneusId",
                table: "AcessoriosCarros");

            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Suspensaos_SuspensaoId",
                table: "AcessoriosCarros");

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

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_CambioId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_CapotaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_CarroceriaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_MotorId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_RodasPneusId",
                table: "AcessoriosCarros");

            migrationBuilder.DropIndex(
                name: "IX_AcessoriosCarros_SuspensaoId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "CambioId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "CapotaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "CarroceriaId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "MotorId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "RodasPneusId",
                table: "AcessoriosCarros");

            migrationBuilder.DropColumn(
                name: "SuspensaoId",
                table: "AcessoriosCarros");

            migrationBuilder.AddColumn<string>(
                name: "Cambio",
                table: "AcessoriosCarros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Capota",
                table: "AcessoriosCarros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Carroceria",
                table: "AcessoriosCarros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motor",
                table: "AcessoriosCarros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RodasPneus",
                table: "AcessoriosCarros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Suspensao",
                table: "AcessoriosCarros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
