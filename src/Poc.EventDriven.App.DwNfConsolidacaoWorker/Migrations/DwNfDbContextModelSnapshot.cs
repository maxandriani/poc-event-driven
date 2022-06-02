﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Poc.EventDriven.Data;

#nullable disable

namespace Poc.EventDriven.Migrations
{
    [DbContext(typeof(DwNfDbContext))]
    partial class DwNfDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Poc.EventDriven.DwNf.DimEmpresa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DimEmpresas");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.DimTempo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Ano")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Dia")
                        .HasColumnType("integer");

                    b.Property<int>("Mes")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DimTempo");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.DimTipoOperacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TipoOperacao")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DimTipoOperacoes");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.FactNf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BlobAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("Chave")
                        .HasColumnType("uuid");

                    b.Property<int>("DimEmissaoId")
                        .HasColumnType("integer");

                    b.Property<int>("DimEmissorId")
                        .HasColumnType("integer");

                    b.Property<int>("DimEmpresaId")
                        .HasColumnType("integer");

                    b.Property<int?>("DimExportadorId")
                        .HasColumnType("integer");

                    b.Property<int>("DimOperacaoId")
                        .HasColumnType("integer");

                    b.Property<int>("Numero")
                        .HasColumnType("integer");

                    b.Property<int>("Serie")
                        .HasColumnType("integer");

                    b.Property<int>("ValorCofinsBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIcmsBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIcmsstBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIiBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIpiBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorPisBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorTotalBrl")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DimEmissaoId");

                    b.HasIndex("DimEmissorId");

                    b.HasIndex("DimEmpresaId");

                    b.HasIndex("DimExportadorId");

                    b.HasIndex("DimOperacaoId");

                    b.ToTable("FactNfs");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.Items.DimNf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BlobAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("Chave")
                        .HasColumnType("uuid");

                    b.Property<int>("Numero")
                        .HasColumnType("integer");

                    b.Property<int>("Serie")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("DimNfs");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.Items.DimSku", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DimSkus");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.Items.FactNfItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DimEmissaoId")
                        .HasColumnType("integer");

                    b.Property<int>("DimEmissorId")
                        .HasColumnType("integer");

                    b.Property<int>("DimEmpresaId")
                        .HasColumnType("integer");

                    b.Property<int?>("DimExportadorId")
                        .HasColumnType("integer");

                    b.Property<int>("DimNfId")
                        .HasColumnType("integer");

                    b.Property<int>("DimSkuId")
                        .HasColumnType("integer");

                    b.Property<int>("DimTipoOperacaoId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantidade")
                        .HasColumnType("integer");

                    b.Property<int>("ValorBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorCofinsBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIcmsBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIcmsstBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIiBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorIpiBrl")
                        .HasColumnType("integer");

                    b.Property<int>("ValorPisBrl")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DimEmissaoId");

                    b.HasIndex("DimEmissorId");

                    b.HasIndex("DimEmpresaId");

                    b.HasIndex("DimExportadorId");

                    b.HasIndex("DimNfId");

                    b.HasIndex("DimSkuId");

                    b.HasIndex("DimTipoOperacaoId");

                    b.ToTable("FactNfItems");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.FactNf", b =>
                {
                    b.HasOne("Poc.EventDriven.DwNf.DimTempo", "DimEmissao")
                        .WithMany()
                        .HasForeignKey("DimEmissaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimEmpresa", "DimEmissor")
                        .WithMany()
                        .HasForeignKey("DimEmissorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimEmpresa", "DimEmpresa")
                        .WithMany()
                        .HasForeignKey("DimEmpresaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimEmpresa", "DimExportador")
                        .WithMany()
                        .HasForeignKey("DimExportadorId");

                    b.HasOne("Poc.EventDriven.DwNf.DimTipoOperacao", "DimOperacao")
                        .WithMany()
                        .HasForeignKey("DimOperacaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DimEmissao");

                    b.Navigation("DimEmissor");

                    b.Navigation("DimEmpresa");

                    b.Navigation("DimExportador");

                    b.Navigation("DimOperacao");
                });

            modelBuilder.Entity("Poc.EventDriven.DwNf.Items.FactNfItem", b =>
                {
                    b.HasOne("Poc.EventDriven.DwNf.DimTempo", "DimEmissao")
                        .WithMany()
                        .HasForeignKey("DimEmissaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimEmpresa", "DimEmissor")
                        .WithMany()
                        .HasForeignKey("DimEmissorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimEmpresa", "DimEmpresa")
                        .WithMany()
                        .HasForeignKey("DimEmpresaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimEmpresa", "DimExportador")
                        .WithMany()
                        .HasForeignKey("DimExportadorId");

                    b.HasOne("Poc.EventDriven.DwNf.Items.DimNf", "DimNf")
                        .WithMany()
                        .HasForeignKey("DimNfId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.Items.DimSku", "DimSku")
                        .WithMany()
                        .HasForeignKey("DimSkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poc.EventDriven.DwNf.DimTipoOperacao", "DimTipoOperacao")
                        .WithMany()
                        .HasForeignKey("DimTipoOperacaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DimEmissao");

                    b.Navigation("DimEmissor");

                    b.Navigation("DimEmpresa");

                    b.Navigation("DimExportador");

                    b.Navigation("DimNf");

                    b.Navigation("DimSku");

                    b.Navigation("DimTipoOperacao");
                });
#pragma warning restore 612, 618
        }
    }
}
