using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Poc.EventDriven.Migrations
{
    public partial class Setup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DimEmpresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimEmpresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimNfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Serie = table.Column<int>(type: "integer", nullable: false),
                    BlobAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimNfs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimSkus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sku = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimSkus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimTempo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dia = table.Column<int>(type: "integer", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimTempo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimTipoOperacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoOperacao = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimTipoOperacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactNfItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DimNfId = table.Column<int>(type: "integer", nullable: false),
                    DimSkuId = table.Column<int>(type: "integer", nullable: false),
                    DimEmpresaId = table.Column<int>(type: "integer", nullable: false),
                    DimExportadorId = table.Column<int>(type: "integer", nullable: true),
                    DimEmissorId = table.Column<int>(type: "integer", nullable: false),
                    DimEmissaoId = table.Column<int>(type: "integer", nullable: false),
                    DimTipoOperacaoId = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ValorBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIcmsBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIcmsstBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIiBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIpiBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorPisBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorCofinsBrl = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactNfItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimEmpresas_DimEmissorId",
                        column: x => x.DimEmissorId,
                        principalTable: "DimEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimEmpresas_DimEmpresaId",
                        column: x => x.DimEmpresaId,
                        principalTable: "DimEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimEmpresas_DimExportadorId",
                        column: x => x.DimExportadorId,
                        principalTable: "DimEmpresas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimNfs_DimNfId",
                        column: x => x.DimNfId,
                        principalTable: "DimNfs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimSkus_DimSkuId",
                        column: x => x.DimSkuId,
                        principalTable: "DimSkus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimTempo_DimEmissaoId",
                        column: x => x.DimEmissaoId,
                        principalTable: "DimTempo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfItems_DimTipoOperacoes_DimTipoOperacaoId",
                        column: x => x.DimTipoOperacaoId,
                        principalTable: "DimTipoOperacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactNfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DimEmpresaId = table.Column<int>(type: "integer", nullable: false),
                    DimExportadorId = table.Column<int>(type: "integer", nullable: true),
                    DimEmissorId = table.Column<int>(type: "integer", nullable: false),
                    DimEmissaoId = table.Column<int>(type: "integer", nullable: false),
                    DimOperacaoId = table.Column<int>(type: "integer", nullable: false),
                    Chave = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Serie = table.Column<int>(type: "integer", nullable: false),
                    BlobAddress = table.Column<string>(type: "text", nullable: false),
                    ValorTotalBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIcmsBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIcmsstBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIiBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorIpiBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorPisBrl = table.Column<int>(type: "integer", nullable: false),
                    ValorCofinsBrl = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactNfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactNfs_DimEmpresas_DimEmissorId",
                        column: x => x.DimEmissorId,
                        principalTable: "DimEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfs_DimEmpresas_DimEmpresaId",
                        column: x => x.DimEmpresaId,
                        principalTable: "DimEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfs_DimEmpresas_DimExportadorId",
                        column: x => x.DimExportadorId,
                        principalTable: "DimEmpresas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FactNfs_DimTempo_DimEmissaoId",
                        column: x => x.DimEmissaoId,
                        principalTable: "DimTempo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactNfs_DimTipoOperacoes_DimOperacaoId",
                        column: x => x.DimOperacaoId,
                        principalTable: "DimTipoOperacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimEmissaoId",
                table: "FactNfItems",
                column: "DimEmissaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimEmissorId",
                table: "FactNfItems",
                column: "DimEmissorId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimEmpresaId",
                table: "FactNfItems",
                column: "DimEmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimExportadorId",
                table: "FactNfItems",
                column: "DimExportadorId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimNfId",
                table: "FactNfItems",
                column: "DimNfId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimSkuId",
                table: "FactNfItems",
                column: "DimSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfItems_DimTipoOperacaoId",
                table: "FactNfItems",
                column: "DimTipoOperacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfs_DimEmissaoId",
                table: "FactNfs",
                column: "DimEmissaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfs_DimEmissorId",
                table: "FactNfs",
                column: "DimEmissorId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfs_DimEmpresaId",
                table: "FactNfs",
                column: "DimEmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfs_DimExportadorId",
                table: "FactNfs",
                column: "DimExportadorId");

            migrationBuilder.CreateIndex(
                name: "IX_FactNfs_DimOperacaoId",
                table: "FactNfs",
                column: "DimOperacaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactNfItems");

            migrationBuilder.DropTable(
                name: "FactNfs");

            migrationBuilder.DropTable(
                name: "DimNfs");

            migrationBuilder.DropTable(
                name: "DimSkus");

            migrationBuilder.DropTable(
                name: "DimEmpresas");

            migrationBuilder.DropTable(
                name: "DimTempo");

            migrationBuilder.DropTable(
                name: "DimTipoOperacoes");
        }
    }
}
