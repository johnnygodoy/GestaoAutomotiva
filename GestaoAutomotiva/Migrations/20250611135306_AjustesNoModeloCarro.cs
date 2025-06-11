using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class AjustesNoModeloCarro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                table: "Carros");

            migrationBuilder.AlterColumn<int>(
                name: "AcessoriosCarroId",
                table: "Carros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                table: "Carros",
                column: "AcessoriosCarroId",
                principalTable: "AcessoriosCarros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                table: "Carros");

            migrationBuilder.AlterColumn<int>(
                name: "AcessoriosCarroId",
                table: "Carros",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Carros_AcessoriosCarros_AcessoriosCarroId",
                table: "Carros",
                column: "AcessoriosCarroId",
                principalTable: "AcessoriosCarros",
                principalColumn: "Id");
        }
    }
}
