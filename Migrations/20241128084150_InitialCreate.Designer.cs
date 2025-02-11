﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Localizard.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Localizard.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241128084150_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LanguageProjectInfo", b =>
                {
                    b.Property<int>("LanguagesId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectInfosId")
                        .HasColumnType("integer");

                    b.HasKey("LanguagesId", "ProjectInfosId");

                    b.HasIndex("ProjectInfosId");

                    b.ToTable("LanguageProjectInfo");
                });

            modelBuilder.Entity("Localizard.Data.Entites.Translation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<string>("SymbolKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("Translations");
                });

            modelBuilder.Entity("Localizard.Domain.Entites.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("LanguageCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Localizard.Domain.Entites.ProjectDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlatformCategories")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectInfoId")
                        .HasColumnType("integer");

                    b.Property<List<int>>("TagIds")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<int>("TranslationId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ProjectDetails");
                });

            modelBuilder.Entity("Localizard.Domain.Entites.ProjectInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ProjectDetailId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectDetailId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Localizard.Domain.Entites.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProjectDetailTranslation", b =>
                {
                    b.Property<int>("ProjectDetailsId")
                        .HasColumnType("integer");

                    b.Property<int>("TranslationId")
                        .HasColumnType("integer");

                    b.HasKey("ProjectDetailsId", "TranslationId");

                    b.HasIndex("TranslationId");

                    b.ToTable("ProjectDetailTranslation");
                });

            modelBuilder.Entity("LanguageProjectInfo", b =>
                {
                    b.HasOne("Localizard.Domain.Entites.Language", null)
                        .WithMany()
                        .HasForeignKey("LanguagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Localizard.Domain.Entites.ProjectInfo", null)
                        .WithMany()
                        .HasForeignKey("ProjectInfosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Localizard.Data.Entites.Translation", b =>
                {
                    b.HasOne("Localizard.Domain.Entites.Language", "Language")
                        .WithMany("Translations")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");
                });

            modelBuilder.Entity("Localizard.Domain.Entites.ProjectInfo", b =>
                {
                    b.HasOne("Localizard.Domain.Entites.ProjectDetail", null)
                        .WithMany("ProjectInfo")
                        .HasForeignKey("ProjectDetailId");
                });

            modelBuilder.Entity("ProjectDetailTranslation", b =>
                {
                    b.HasOne("Localizard.Domain.Entites.ProjectDetail", null)
                        .WithMany()
                        .HasForeignKey("ProjectDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Localizard.Data.Entites.Translation", null)
                        .WithMany()
                        .HasForeignKey("TranslationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Localizard.Domain.Entites.Language", b =>
                {
                    b.Navigation("Translations");
                });

            modelBuilder.Entity("Localizard.Domain.Entites.ProjectDetail", b =>
                {
                    b.Navigation("ProjectInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
