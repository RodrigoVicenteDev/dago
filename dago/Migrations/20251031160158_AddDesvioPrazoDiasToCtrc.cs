using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddDesvioPrazoDiasToCtrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesvioPrazoDias",
                table: "Ctrcs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Ctrcs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesvioPrazoDias",
                table: "Ctrcs");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Ctrcs");
        }
    }
}
