using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAutomotiva.Migrations
{
    /// <inheritdoc />
    public partial class MotorIdOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Motors_MotorId",
                table: "AcessoriosCarros");

            migrationBuilder.AlterColumn<int>(
                name: "MotorId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Motors_MotorId",
                table: "AcessoriosCarros",
                column: "MotorId",
                principalTable: "Motors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessoriosCarros_Motors_MotorId",
                table: "AcessoriosCarros");

            migrationBuilder.AlterColumn<int>(
                name: "MotorId",
                table: "AcessoriosCarros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AcessoriosCarros_Motors_MotorId",
                table: "AcessoriosCarros",
                column: "MotorId",
                principalTable: "Motors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
