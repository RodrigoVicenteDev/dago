using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class EsporadicoConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "OcorrenciasSistema",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "OcorrenciasAtendimento",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataPrevistaEntrega",
                table: "Ctrcs",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEmissao",
                table: "Ctrcs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

           
            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Agendas",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "ConfiguracoesEsporadico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesEsporadico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracaoEsporadicoClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfiguracaoEsporadicoId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoEsporadicoClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoEsporadicoClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoEsporadicoClientes_ConfiguracoesEsporadico_Conf~",
                        column: x => x.ConfiguracaoEsporadicoId,
                        principalTable: "ConfiguracoesEsporadico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracaoEsporadicoDestinatarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfiguracaoEsporadicoId = table.Column<int>(type: "integer", nullable: false),
                    Destinatario = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoEsporadicoDestinatarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoEsporadicoDestinatarios_ConfiguracoesEsporadico~",
                        column: x => x.ConfiguracaoEsporadicoId,
                        principalTable: "ConfiguracoesEsporadico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracaoEsporadicoUnidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfiguracaoEsporadicoId = table.Column<int>(type: "integer", nullable: false),
                    UnidadeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoEsporadicoUnidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoEsporadicoUnidades_ConfiguracoesEsporadico_Conf~",
                        column: x => x.ConfiguracaoEsporadicoId,
                        principalTable: "ConfiguracoesEsporadico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoEsporadicoUnidades_Unidades_UnidadeId",
                        column: x => x.UnidadeId,
                        principalTable: "Unidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoEsporadicoClientes_ClienteId",
                table: "ConfiguracaoEsporadicoClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoEsporadicoClientes_ConfiguracaoEsporadicoId",
                table: "ConfiguracaoEsporadicoClientes",
                column: "ConfiguracaoEsporadicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoEsporadicoDestinatarios_ConfiguracaoEsporadicoId",
                table: "ConfiguracaoEsporadicoDestinatarios",
                column: "ConfiguracaoEsporadicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoEsporadicoUnidades_ConfiguracaoEsporadicoId",
                table: "ConfiguracaoEsporadicoUnidades",
                column: "ConfiguracaoEsporadicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoEsporadicoUnidades_UnidadeId",
                table: "ConfiguracaoEsporadicoUnidades",
                column: "UnidadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoEsporadicoClientes");

            migrationBuilder.DropTable(
                name: "ConfiguracaoEsporadicoDestinatarios");

            migrationBuilder.DropTable(
                name: "ConfiguracaoEsporadicoUnidades");

            migrationBuilder.DropTable(
                name: "ConfiguracoesEsporadico");

            migrationBuilder.DropColumn(
                name: "DataEntregaRealizada",
                table: "Ctrcs");

            migrationBuilder.DropColumn(
                name: "NotasFiscais",
                table: "Ctrcs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "OcorrenciasSistema",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "OcorrenciasAtendimento",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataPrevistaEntrega",
                table: "Ctrcs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEmissao",
                table: "Ctrcs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Agendas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
