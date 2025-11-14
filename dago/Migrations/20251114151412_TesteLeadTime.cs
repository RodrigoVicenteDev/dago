using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dago.Migrations
{
    public partial class TesteLeadTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //
            // REMOVER FKs FANTASMAS – APENAS SE EXISTIREM
            //
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'FK_LeadTimesCliente_Cidades_CidadeId1'
    ) THEN
        ALTER TABLE ""LeadTimesCliente"" DROP CONSTRAINT ""FK_LeadTimesCliente_Cidades_CidadeId1"";
    END IF;
END$$;");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'FK_LeadTimesCliente_Clientes_ClienteId1'
    ) THEN
        ALTER TABLE ""LeadTimesCliente"" DROP CONSTRAINT ""FK_LeadTimesCliente_Clientes_ClienteId1"";
    END IF;
END$$;");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'FK_LeadTimesCliente_Estados_EstadoId1'
    ) THEN
        ALTER TABLE ""LeadTimesCliente"" DROP CONSTRAINT ""FK_LeadTimesCliente_Estados_EstadoId1"";
    END IF;
END$$;");

            //
            // REMOVER ÍNDICES FANTASMAS – APENAS SE EXISTIREM
            //
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE indexname = 'IX_LeadTimesCliente_CidadeId1'
    ) THEN
        DROP INDEX ""IX_LeadTimesCliente_CidadeId1"";
    END IF;
END$$;");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE indexname = 'IX_LeadTimesCliente_ClienteId1'
    ) THEN
        DROP INDEX ""IX_LeadTimesCliente_ClienteId1"";
    END IF;
END$$;");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE indexname = 'IX_LeadTimesCliente_EstadoId1'
    ) THEN
        DROP INDEX ""IX_LeadTimesCliente_EstadoId1"";
    END IF;
END$$;");

            //
            // REMOVER COLUNAS FANTASMAS – APENAS SE EXISTIREM
            //
            migrationBuilder.Sql(@"ALTER TABLE ""LeadTimesCliente"" DROP COLUMN IF EXISTS ""CidadeId1"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LeadTimesCliente"" DROP COLUMN IF EXISTS ""ClienteId1"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LeadTimesCliente"" DROP COLUMN IF EXISTS ""EstadoId1"";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CidadeId1",
                table: "LeadTimesCliente",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "LeadTimesCliente",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId1",
                table: "LeadTimesCliente",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadTimesCliente_CidadeId1",
                table: "LeadTimesCliente",
                column: "CidadeId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTimesCliente_ClienteId1",
                table: "LeadTimesCliente",
                column: "ClienteId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTimesCliente_EstadoId1",
                table: "LeadTimesCliente",
                column: "EstadoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_Cidades_CidadeId1",
                table: "LeadTimesCliente",
                column: "CidadeId1",
                principalTable: "Cidades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_Clientes_ClienteId1",
                table: "LeadTimesCliente",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadTimesCliente_Estados_EstadoId1",
                table: "LeadTimesCliente",
                column: "EstadoId1",
                principalTable: "Estados",
                principalColumn: "Id");
        }
    }
}
