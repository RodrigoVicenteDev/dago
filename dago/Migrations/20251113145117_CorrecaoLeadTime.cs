using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    public partial class CorrecaoLeadTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Remove colunas antigas SE EXISTIREM
            migrationBuilder.DropColumn(
                name: "TipoRegiaoId",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "RegiaoEstadoId",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "CidadeId1",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "EstadoId1",
                table: "LeadTimesCliente");

            // 2️⃣ Cria colunas CORRETAS caso não existam
            migrationBuilder.AddColumn<int>(
                name: "CidadeId",
                table: "LeadTimesCliente",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "LeadTimesCliente",
                nullable: false,
                defaultValue: 0);

            // 3️⃣ Recria as chaves estrangeiras CORRETAS
            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_Clientes_ClienteId",
                table: "LeadTimesCliente",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_Cidades_CidadeId",
                table: "LeadTimesCliente",
                column: "CidadeId",
                principalTable: "Cidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_Estados_EstadoId",
                table: "LeadTimesCliente",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverte mudanças
            migrationBuilder.DropForeignKey(
                name: "FK_LeadTimesCliente_Clientes_ClienteId",
                table: "LeadTimesCliente");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadTimesCliente_Cidades_CidadeId",
                table: "LeadTimesCliente");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadTimesCliente_Estados_EstadoId",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "CidadeId",
                table: "LeadTimesCliente");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "LeadTimesCliente");

            // recria colunas antigas se necessário
            migrationBuilder.AddColumn<int>(
                name: "TipoRegiaoId",
                table: "LeadTimesCliente",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegiaoEstadoId",
                table: "LeadTimesCliente",
                type: "integer",
                nullable: true);
        }
    }
}
