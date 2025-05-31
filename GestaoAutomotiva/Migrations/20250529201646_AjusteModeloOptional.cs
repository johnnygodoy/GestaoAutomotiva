using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjusteModeloOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RodasPneus_Modelos_ModeloId",
                table: "RodasPneus");

            migrationBuilder.AlterColumn<int>(
                name: "ModeloId",
                table: "Suspensaos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ModeloId",
                table: "RodasPneus",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_RodasPneus_Modelos_ModeloId",
                table: "RodasPneus",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RodasPneus_Modelos_ModeloId",
                table: "RodasPneus");

            migrationBuilder.AlterColumn<int>(
                name: "ModeloId",
                table: "Suspensaos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModeloId",
                table: "RodasPneus",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RodasPneus_Modelos_ModeloId",
                table: "RodasPneus",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
