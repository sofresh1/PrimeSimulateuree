﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PrimeSimulateur.Models;

namespace PrimeSimulateur.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20200818115519_add-table-trace-to-MyDB11")]
    partial class addtabletracetoMyDB11
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PrimeSimulateur.Models.Categorie", b =>
                {
                    b.Property<int>("CategorieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TravailId")
                        .HasColumnType("int");

                    b.Property<string>("type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategorieId");

                    b.HasIndex("TravailId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("firstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClientId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Logement", b =>
                {
                    b.Property<int>("LogementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("TypeEnergie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ville")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("adresse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("surface")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LogementId");

                    b.HasIndex("ClientId");

                    b.ToTable("Logements");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.ProjectRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProjectRole");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Situation", b =>
                {
                    b.Property<int>("SituationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<int>("Nombredepersonne")
                        .HasColumnType("int");

                    b.Property<float>("Revenumenage")
                        .HasColumnType("real");

                    b.HasKey("SituationId");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("Situations");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Travail", b =>
                {
                    b.Property<int>("TravailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LogementId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("surface")
                        .HasColumnType("real");

                    b.HasKey("TravailId");

                    b.HasIndex("LogementId");

                    b.ToTable("Travails");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.trace", b =>
                {
                    b.Property<int>("traceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("Nom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Surface")
                        .HasColumnType("real");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("prime")
                        .HasColumnType("real");

                    b.HasKey("traceId");

                    b.HasIndex("ClientId");

                    b.ToTable("trace");
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Categorie", b =>
                {
                    b.HasOne("PrimeSimulateur.Models.Travail", "Travail")
                        .WithMany("Categories")
                        .HasForeignKey("TravailId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Logement", b =>
                {
                    b.HasOne("PrimeSimulateur.Models.Client", "Client")
                        .WithMany("Logements")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Situation", b =>
                {
                    b.HasOne("PrimeSimulateur.Models.Client", "Client")
                        .WithOne("Situation")
                        .HasForeignKey("PrimeSimulateur.Models.Situation", "ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PrimeSimulateur.Models.Travail", b =>
                {
                    b.HasOne("PrimeSimulateur.Models.Logement", "Logement")
                        .WithMany("Travails")
                        .HasForeignKey("LogementId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PrimeSimulateur.Models.trace", b =>
                {
                    b.HasOne("PrimeSimulateur.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
