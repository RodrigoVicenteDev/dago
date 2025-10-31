using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddDestinatariokOnCtrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destinatario",
                table: "Ctrcs",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destinatario",
                table: "Ctrcs");
        }
    }
}
