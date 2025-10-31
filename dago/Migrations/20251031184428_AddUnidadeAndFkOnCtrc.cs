using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddUnidadeAndFkOnCtrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnidadeId",
                table: "Ctrcs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Unidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    EstadoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unidades_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ctrcs_UnidadeId",
                table: "Ctrcs",
                column: "UnidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_EstadoId",
                table: "Unidades",
                column: "EstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ctrcs_Unidades_UnidadeId",
                table: "Ctrcs",
                column: "UnidadeId",
                principalTable: "Unidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ctrcs_Unidades_UnidadeId",
                table: "Ctrcs");

            migrationBuilder.DropTable(
                name: "Unidades");

            migrationBuilder.DropIndex(
                name: "IX_Ctrcs_UnidadeId",
                table: "Ctrcs");

            migrationBuilder.DropColumn(
                name: "UnidadeId",
                table: "Ctrcs");
        }
    }
}
