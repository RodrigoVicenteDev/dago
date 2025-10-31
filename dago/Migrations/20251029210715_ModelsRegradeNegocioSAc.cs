using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace dago.Migrations
{
    /// <inheritdoc />
    public partial class ModelsRegradeNegocioSAc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Cargos_CargoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoUf = table.Column<int>(type: "integer", nullable: false),
                    Sigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusesEntrega",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusesEntrega", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposAgenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAgenda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposOcorrencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposOcorrencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposRegiao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposRegiao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EstadoId = table.Column<int>(type: "integer", nullable: false),
                    TipoRegiaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cidades_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cidades_TiposRegiao_TipoRegiaoId",
                        column: x => x.TipoRegiaoId,
                        principalTable: "TiposRegiao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadTimesCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    TipoRegiaoId = table.Column<int>(type: "integer", nullable: false),
                    DiasLead = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadTimesCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadTimesCliente_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadTimesCliente_TiposRegiao_TipoRegiaoId",
                        column: x => x.TipoRegiaoId,
                        principalTable: "TiposRegiao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ctrcs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroNotaFiscal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    CidadeDestinoId = table.Column<int>(type: "integer", nullable: false),
                    EstadoDestinoId = table.Column<int>(type: "integer", nullable: false),
                    StatusEntregaId = table.Column<int>(type: "integer", nullable: false),
                    LeadTimeDias = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ctrcs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ctrcs_Cidades_CidadeDestinoId",
                        column: x => x.CidadeDestinoId,
                        principalTable: "Cidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctrcs_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctrcs_Estados_EstadoDestinoId",
                        column: x => x.EstadoDestinoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctrcs_StatusesEntrega_StatusEntregaId",
                        column: x => x.StatusEntregaId,
                        principalTable: "StatusesEntrega",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CtrcId = table.Column<int>(type: "integer", nullable: false),
                    TipoAgendaId = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendas_Ctrcs_CtrcId",
                        column: x => x.CtrcId,
                        principalTable: "Ctrcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendas_TiposAgenda_TipoAgendaId",
                        column: x => x.TipoAgendaId,
                        principalTable: "TiposAgenda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OcorrenciasAtendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CtrcId = table.Column<int>(type: "integer", nullable: false),
                    TipoOcorrenciaId = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ReplicaClientes = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcorrenciasAtendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcorrenciasAtendimento_Ctrcs_CtrcId",
                        column: x => x.CtrcId,
                        principalTable: "Ctrcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OcorrenciasAtendimento_TiposOcorrencia_TipoOcorrenciaId",
                        column: x => x.TipoOcorrenciaId,
                        principalTable: "TiposOcorrencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OcorrenciasSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CtrcId = table.Column<int>(type: "integer", nullable: false),
                    NumeroOcorrencia = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcorrenciasSistema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcorrenciasSistema_Ctrcs_CtrcId",
                        column: x => x.CtrcId,
                        principalTable: "Ctrcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticularidadesCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CtrcId = table.Column<int>(type: "integer", nullable: false),
                    Remessa = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Loja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticularidadesCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticularidadesCliente_Ctrcs_CtrcId",
                        column: x => x.CtrcId,
                        principalTable: "Ctrcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_CtrcId",
                table: "Agendas",
                column: "CtrcId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_TipoAgendaId",
                table: "Agendas",
                column: "TipoAgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cidades_EstadoId",
                table: "Cidades",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cidades_TipoRegiaoId",
                table: "Cidades",
                column: "TipoRegiaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctrcs_CidadeDestinoId",
                table: "Ctrcs",
                column: "CidadeDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctrcs_ClienteId",
                table: "Ctrcs",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctrcs_EstadoDestinoId",
                table: "Ctrcs",
                column: "EstadoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctrcs_StatusEntregaId",
                table: "Ctrcs",
                column: "StatusEntregaId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTimesCliente_ClienteId",
                table: "LeadTimesCliente",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadTimesCliente_TipoRegiaoId",
                table: "LeadTimesCliente",
                column: "TipoRegiaoId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasAtendimento_CtrcId",
                table: "OcorrenciasAtendimento",
                column: "CtrcId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasAtendimento_TipoOcorrenciaId",
                table: "OcorrenciasAtendimento",
                column: "TipoOcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasSistema_CtrcId",
                table: "OcorrenciasSistema",
                column: "CtrcId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticularidadesCliente_CtrcId",
                table: "ParticularidadesCliente",
                column: "CtrcId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Cargos_CargoId",
                table: "Usuarios",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Cargos_CargoId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Agendas");

            migrationBuilder.DropTable(
                name: "LeadTimesCliente");

            migrationBuilder.DropTable(
                name: "OcorrenciasAtendimento");

            migrationBuilder.DropTable(
                name: "OcorrenciasSistema");

            migrationBuilder.DropTable(
                name: "ParticularidadesCliente");

            migrationBuilder.DropTable(
                name: "TiposAgenda");

            migrationBuilder.DropTable(
                name: "TiposOcorrencia");

            migrationBuilder.DropTable(
                name: "Ctrcs");

            migrationBuilder.DropTable(
                name: "Cidades");

            migrationBuilder.DropTable(
                name: "StatusesEntrega");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "TiposRegiao");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Cargos_CargoId",
                table: "Usuarios",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
