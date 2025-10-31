using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddRegiao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Regiao",
                table: "Estados",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RegiaoEstadoId",
                table: "Estados",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegioesEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Sigla = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegioesEstados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estados_RegiaoEstadoId",
                table: "Estados",
                column: "RegiaoEstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estados_RegioesEstados_RegiaoEstadoId",
                table: "Estados",
                column: "RegiaoEstadoId",
                principalTable: "RegioesEstados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estados_RegioesEstados_RegiaoEstadoId",
                table: "Estados");

            migrationBuilder.DropTable(
                name: "RegioesEstados");

            migrationBuilder.DropIndex(
                name: "IX_Estados_RegiaoEstadoId",
                table: "Estados");

            migrationBuilder.DropColumn(
                name: "Regiao",
                table: "Estados");

            migrationBuilder.DropColumn(
                name: "RegiaoEstadoId",
                table: "Estados");
        }
    }
}
