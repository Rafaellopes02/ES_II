using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrabalhoESII.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    idcategoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.idcategoria);
                });

            migrationBuilder.CreateTable(
                name: "estadospagamentos",
                columns: table => new
                {
                    idestado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estadospagamentos", x => x.idestado);
                });

            migrationBuilder.CreateTable(
                name: "tiposingressos",
                columns: table => new
                {
                    idtipoingresso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tiposingressos", x => x.idtipoingresso);
                });

            migrationBuilder.CreateTable(
                name: "tipospagamentos",
                columns: table => new
                {
                    idtipopagamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipospagamentos", x => x.idtipopagamento);
                });

            migrationBuilder.CreateTable(
                name: "tiposutilizadores",
                columns: table => new
                {
                    idtipoutilizador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tiposutilizadores", x => x.idtipoutilizador);
                });

            migrationBuilder.CreateTable(
                name: "eventos",
                columns: table => new
                {
                    idevento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora = table.Column<TimeSpan>(type: "interval", nullable: false),
                    local = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    capacidade = table.Column<int>(type: "integer", nullable: false),
                    idcategoria = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos", x => x.idevento);
                    table.ForeignKey(
                        name: "FK_eventos_categorias_idcategoria",
                        column: x => x.idcategoria,
                        principalTable: "categorias",
                        principalColumn: "idcategoria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "utilizadores",
                columns: table => new
                {
                    idutilizador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    idade = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    telefone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    nacionalidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nomeutilizador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    senha = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    idtipoutilizador = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilizadores", x => x.idutilizador);
                    table.ForeignKey(
                        name: "FK_utilizadores_tiposutilizadores_idtipoutilizador",
                        column: x => x.idtipoutilizador,
                        principalTable: "tiposutilizadores",
                        principalColumn: "idtipoutilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "atividades",
                columns: table => new
                {
                    idatividade = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    quantidademaxima = table.Column<int>(type: "integer", nullable: true),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora = table.Column<TimeSpan>(type: "interval", nullable: false),
                    idevento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atividades", x => x.idatividade);
                    table.ForeignKey(
                        name: "FK_atividades_eventos_idevento",
                        column: x => x.idevento,
                        principalTable: "eventos",
                        principalColumn: "idevento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ingressos",
                columns: table => new
                {
                    idingresso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    quantidadeatual = table.Column<int>(type: "integer", nullable: false),
                    quantidadedefinida = table.Column<int>(type: "integer", nullable: false),
                    idevento = table.Column<int>(type: "integer", nullable: false),
                    idtipoingresso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingressos", x => x.idingresso);
                    table.ForeignKey(
                        name: "FK_ingressos_eventos_idevento",
                        column: x => x.idevento,
                        principalTable: "eventos",
                        principalColumn: "idevento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ingressos_tiposingressos_idtipoingresso",
                        column: x => x.idtipoingresso,
                        principalTable: "tiposingressos",
                        principalColumn: "idtipoingresso",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    idfeedback = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    avaliacao = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    idutilizador = table.Column<int>(type: "integer", nullable: false),
                    idevento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => x.idfeedback);
                    table.ForeignKey(
                        name: "FK_feedbacks_eventos_idevento",
                        column: x => x.idevento,
                        principalTable: "eventos",
                        principalColumn: "idevento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feedbacks_utilizadores_idutilizador",
                        column: x => x.idutilizador,
                        principalTable: "utilizadores",
                        principalColumn: "idutilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organizadoreseventos",
                columns: table => new
                {
                    idutilizador = table.Column<int>(type: "integer", nullable: false),
                    idevento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizadoreseventos", x => new { x.idutilizador, x.idevento });
                    table.ForeignKey(
                        name: "FK_organizadoreseventos_eventos_idevento",
                        column: x => x.idevento,
                        principalTable: "eventos",
                        principalColumn: "idevento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organizadoreseventos_utilizadores_idutilizador",
                        column: x => x.idutilizador,
                        principalTable: "utilizadores",
                        principalColumn: "idutilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "utilizadoresatividades",
                columns: table => new
                {
                    idutilizador = table.Column<int>(type: "integer", nullable: false),
                    idatividade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilizadoresatividades", x => new { x.idutilizador, x.idatividade });
                    table.ForeignKey(
                        name: "FK_utilizadoresatividades_atividades_idatividade",
                        column: x => x.idatividade,
                        principalTable: "atividades",
                        principalColumn: "idatividade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_utilizadoresatividades_utilizadores_idutilizador",
                        column: x => x.idutilizador,
                        principalTable: "utilizadores",
                        principalColumn: "idutilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    idpagamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    datahora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    idtipopagamento = table.Column<int>(type: "integer", nullable: false),
                    idutilizador = table.Column<int>(type: "integer", nullable: false),
                    idingresso = table.Column<int>(type: "integer", nullable: false),
                    idestado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamentos", x => x.idpagamento);
                    table.ForeignKey(
                        name: "FK_pagamentos_estadospagamentos_idestado",
                        column: x => x.idestado,
                        principalTable: "estadospagamentos",
                        principalColumn: "idestado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pagamentos_ingressos_idingresso",
                        column: x => x.idingresso,
                        principalTable: "ingressos",
                        principalColumn: "idingresso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pagamentos_tipospagamentos_idtipopagamento",
                        column: x => x.idtipopagamento,
                        principalTable: "tipospagamentos",
                        principalColumn: "idtipopagamento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pagamentos_utilizadores_idutilizador",
                        column: x => x.idutilizador,
                        principalTable: "utilizadores",
                        principalColumn: "idutilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_atividades_idevento",
                table: "atividades",
                column: "idevento");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_idcategoria",
                table: "eventos",
                column: "idcategoria");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_idevento",
                table: "feedbacks",
                column: "idevento");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_idutilizador",
                table: "feedbacks",
                column: "idutilizador");

            migrationBuilder.CreateIndex(
                name: "IX_ingressos_idevento",
                table: "ingressos",
                column: "idevento");

            migrationBuilder.CreateIndex(
                name: "IX_ingressos_idtipoingresso",
                table: "ingressos",
                column: "idtipoingresso");

            migrationBuilder.CreateIndex(
                name: "IX_organizadoreseventos_idevento",
                table: "organizadoreseventos",
                column: "idevento");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_idestado",
                table: "pagamentos",
                column: "idestado");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_idingresso",
                table: "pagamentos",
                column: "idingresso");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_idtipopagamento",
                table: "pagamentos",
                column: "idtipopagamento");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_idutilizador",
                table: "pagamentos",
                column: "idutilizador");

            migrationBuilder.CreateIndex(
                name: "IX_utilizadores_idtipoutilizador",
                table: "utilizadores",
                column: "idtipoutilizador");

            migrationBuilder.CreateIndex(
                name: "IX_utilizadoresatividades_idatividade",
                table: "utilizadoresatividades",
                column: "idatividade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "organizadoreseventos");

            migrationBuilder.DropTable(
                name: "pagamentos");

            migrationBuilder.DropTable(
                name: "utilizadoresatividades");

            migrationBuilder.DropTable(
                name: "estadospagamentos");

            migrationBuilder.DropTable(
                name: "ingressos");

            migrationBuilder.DropTable(
                name: "tipospagamentos");

            migrationBuilder.DropTable(
                name: "atividades");

            migrationBuilder.DropTable(
                name: "utilizadores");

            migrationBuilder.DropTable(
                name: "tiposingressos");

            migrationBuilder.DropTable(
                name: "eventos");

            migrationBuilder.DropTable(
                name: "tiposutilizadores");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
