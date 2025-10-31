using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddpesoonOnCtrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Peso",
                table: "Ctrcs",
                type: "numeric(10,4)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Ctrcs");
        }
    }
}
