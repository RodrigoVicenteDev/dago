using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddRegiaoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Regiao",
                table: "Estados");

            migrationBuilder.AlterColumn<int>(
                name: "RegiaoEstadoId",
                table: "Estados",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RegiaoEstadoId",
                table: "Estados",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Regiao",
                table: "Estados",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
