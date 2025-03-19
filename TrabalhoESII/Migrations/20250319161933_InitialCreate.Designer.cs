﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrabalhoESII.Models;

#nullable disable

namespace TrabalhoESII.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250319161933_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TrabalhoESII.Models.atividades", b =>
                {
                    b.Property<int>("idatividade")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idatividade"));

                    b.Property<DateTime>("data")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeSpan>("hora")
                        .HasColumnType("interval");

                    b.Property<int>("idevento")
                        .HasColumnType("integer");

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("quantidademaxima")
                        .HasColumnType("integer");

                    b.HasKey("idatividade");

                    b.HasIndex("idevento");

                    b.ToTable("atividades");
                });

            modelBuilder.Entity("TrabalhoESII.Models.categorias", b =>
                {
                    b.Property<int>("idcategoria")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idcategoria"));

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("idcategoria");

                    b.ToTable("categorias");
                });

            modelBuilder.Entity("TrabalhoESII.Models.estadospagamentos", b =>
                {
                    b.Property<int>("idestado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idestado"));

                    b.Property<string>("descricao")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("idestado");

                    b.ToTable("estadospagamentos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.eventos", b =>
                {
                    b.Property<int>("idevento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idevento"));

                    b.Property<int>("capacidade")
                        .HasColumnType("integer");

                    b.Property<DateTime>("data")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("descricao")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<TimeSpan>("hora")
                        .HasColumnType("interval");

                    b.Property<int>("idcategoria")
                        .HasColumnType("integer");

                    b.Property<string>("local")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("idevento");

                    b.HasIndex("idcategoria");

                    b.ToTable("eventos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.feedbacks", b =>
                {
                    b.Property<int>("idfeedback")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idfeedback"));

                    b.Property<int>("avaliacao")
                        .HasColumnType("integer");

                    b.Property<string>("comentario")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("data")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("idevento")
                        .HasColumnType("integer");

                    b.Property<int>("idutilizador")
                        .HasColumnType("integer");

                    b.HasKey("idfeedback");

                    b.HasIndex("idevento");

                    b.HasIndex("idutilizador");

                    b.ToTable("feedbacks");
                });

            modelBuilder.Entity("TrabalhoESII.Models.ingressos", b =>
                {
                    b.Property<int>("idingresso")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idingresso"));

                    b.Property<int>("idevento")
                        .HasColumnType("integer");

                    b.Property<int>("idtipoingresso")
                        .HasColumnType("integer");

                    b.Property<decimal>("preco")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("quantidadeatual")
                        .HasColumnType("integer");

                    b.Property<int>("quantidadedefinida")
                        .HasColumnType("integer");

                    b.HasKey("idingresso");

                    b.HasIndex("idevento");

                    b.HasIndex("idtipoingresso");

                    b.ToTable("ingressos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.organizadoreseventos", b =>
                {
                    b.Property<int>("idutilizador")
                        .HasColumnType("integer");

                    b.Property<int>("idevento")
                        .HasColumnType("integer");

                    b.HasKey("idutilizador", "idevento");

                    b.HasIndex("idevento");

                    b.ToTable("organizadoreseventos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.pagamentos", b =>
                {
                    b.Property<int>("idpagamento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idpagamento"));

                    b.Property<DateTime>("datahora")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("descricao")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("idestado")
                        .HasColumnType("integer");

                    b.Property<int>("idingresso")
                        .HasColumnType("integer");

                    b.Property<int>("idtipopagamento")
                        .HasColumnType("integer");

                    b.Property<int>("idutilizador")
                        .HasColumnType("integer");

                    b.HasKey("idpagamento");

                    b.HasIndex("idestado");

                    b.HasIndex("idingresso");

                    b.HasIndex("idtipopagamento");

                    b.HasIndex("idutilizador");

                    b.ToTable("pagamentos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.tiposingressos", b =>
                {
                    b.Property<int>("idtipoingresso")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idtipoingresso"));

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("idtipoingresso");

                    b.ToTable("tiposingressos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.tipospagamentos", b =>
                {
                    b.Property<int>("idtipopagamento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idtipopagamento"));

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("idtipopagamento");

                    b.ToTable("tipospagamentos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.tiposutilizadores", b =>
                {
                    b.Property<int>("idtipoutilizador")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idtipoutilizador"));

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("idtipoutilizador");

                    b.ToTable("tiposutilizadores");
                });

            modelBuilder.Entity("TrabalhoESII.Models.utilizadores", b =>
                {
                    b.Property<int>("idutilizador")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("idutilizador"));

                    b.Property<string>("email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("idade")
                        .HasColumnType("integer");

                    b.Property<int>("idtipoutilizador")
                        .HasColumnType("integer");

                    b.Property<string>("nacionalidade")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("nomeutilizador")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("senha")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("telefone")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.HasKey("idutilizador");

                    b.HasIndex("idtipoutilizador");

                    b.ToTable("utilizadores");
                });

            modelBuilder.Entity("TrabalhoESII.Models.utilizadoresatividades", b =>
                {
                    b.Property<int>("idutilizador")
                        .HasColumnType("integer");

                    b.Property<int>("idatividade")
                        .HasColumnType("integer");

                    b.HasKey("idutilizador", "idatividade");

                    b.HasIndex("idatividade");

                    b.ToTable("utilizadoresatividades");
                });

            modelBuilder.Entity("TrabalhoESII.Models.atividades", b =>
                {
                    b.HasOne("TrabalhoESII.Models.eventos", "eventos")
                        .WithMany()
                        .HasForeignKey("idevento")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("eventos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.eventos", b =>
                {
                    b.HasOne("TrabalhoESII.Models.categorias", "categoria")
                        .WithMany()
                        .HasForeignKey("idcategoria")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("categoria");
                });

            modelBuilder.Entity("TrabalhoESII.Models.feedbacks", b =>
                {
                    b.HasOne("TrabalhoESII.Models.eventos", "eventos")
                        .WithMany()
                        .HasForeignKey("idevento")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.utilizadores", "utilizadores")
                        .WithMany()
                        .HasForeignKey("idutilizador")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("eventos");

                    b.Navigation("utilizadores");
                });

            modelBuilder.Entity("TrabalhoESII.Models.ingressos", b =>
                {
                    b.HasOne("TrabalhoESII.Models.eventos", "eventos")
                        .WithMany()
                        .HasForeignKey("idevento")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.tiposingressos", "tiposingressos")
                        .WithMany()
                        .HasForeignKey("idtipoingresso")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("eventos");

                    b.Navigation("tiposingressos");
                });

            modelBuilder.Entity("TrabalhoESII.Models.organizadoreseventos", b =>
                {
                    b.HasOne("TrabalhoESII.Models.eventos", "eventos")
                        .WithMany()
                        .HasForeignKey("idevento")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.utilizadores", "utilizadores")
                        .WithMany()
                        .HasForeignKey("idutilizador")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("eventos");

                    b.Navigation("utilizadores");
                });

            modelBuilder.Entity("TrabalhoESII.Models.pagamentos", b =>
                {
                    b.HasOne("TrabalhoESII.Models.estadospagamentos", "estadospagamentos")
                        .WithMany()
                        .HasForeignKey("idestado")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.ingressos", "ingressos")
                        .WithMany()
                        .HasForeignKey("idingresso")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.tipospagamentos", "tipospagamentos")
                        .WithMany()
                        .HasForeignKey("idtipopagamento")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.utilizadores", "utilizadores")
                        .WithMany()
                        .HasForeignKey("idutilizador")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("estadospagamentos");

                    b.Navigation("ingressos");

                    b.Navigation("tipospagamentos");

                    b.Navigation("utilizadores");
                });

            modelBuilder.Entity("TrabalhoESII.Models.utilizadores", b =>
                {
                    b.HasOne("TrabalhoESII.Models.tiposutilizadores", "tiposutilizadores")
                        .WithMany()
                        .HasForeignKey("idtipoutilizador")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("tiposutilizadores");
                });

            modelBuilder.Entity("TrabalhoESII.Models.utilizadoresatividades", b =>
                {
                    b.HasOne("TrabalhoESII.Models.atividades", "atividades")
                        .WithMany()
                        .HasForeignKey("idatividade")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrabalhoESII.Models.utilizadores", "utilizadores")
                        .WithMany()
                        .HasForeignKey("idutilizador")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("atividades");

                    b.Navigation("utilizadores");
                });
#pragma warning restore 612, 618
        }
    }
}
