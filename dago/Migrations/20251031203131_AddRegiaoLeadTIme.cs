using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class AddRegiaoLeadTIme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegiaoEstadoId",
                table: "LeadTimesCliente",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LeadTimesCliente_RegiaoEstadoId",
                table: "LeadTimesCliente",
                column: "RegiaoEstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_RegioesEstados_RegiaoEstadoId",
                table: "LeadTimesCliente",
                column: "RegiaoEstadoId",
                principalTable: "RegioesEstados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadTimesCliente_RegioesEstados_RegiaoEstadoId",
                table: "LeadTimesCliente");

            migrationBuilder.DropIndex(
                name: "IX_LeadTimesCliente_RegiaoEstadoId",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "RegiaoEstadoId",
                table: "LeadTimesCliente");
        }
    }
}
