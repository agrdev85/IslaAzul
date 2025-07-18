﻿// <auto-generated />
using System;
using HostalIslaAzul.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HostalIslaAzul.Migrations
{
    [DbContext(typeof(HostalDbContext))]
    partial class HostalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.AmaDeLlaves", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CI")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NombreApellidos")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumeroTelefonico")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CI")
                        .IsUnique();

                    b.ToTable("AmasDeLlaves");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Cliente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CI")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EsVip")
                        .HasColumnType("bit");

                    b.Property<string>("NombreApellidos")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumeroTelefonico")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Habitacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("EstaFueraDeServicio")
                        .HasColumnType("bit");

                    b.Property<string>("Numero")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Numero")
                        .IsUnique();

                    b.ToTable("Habitaciones");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.HabitacionAmaDeLlaves", b =>
                {
                    b.Property<int>("AmaDeLlavesId")
                        .HasColumnType("int");

                    b.Property<int>("HabitacionId")
                        .HasColumnType("int");

                    b.Property<string>("Turno")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("AmaDeLlavesId1")
                        .HasColumnType("int");

                    b.Property<int?>("HabitacionId1")
                        .HasColumnType("int");

                    b.HasKey("AmaDeLlavesId", "HabitacionId", "Turno");

                    b.HasIndex("AmaDeLlavesId1");

                    b.HasIndex("HabitacionId");

                    b.HasIndex("HabitacionId1");

                    b.ToTable("HabitacionesAmasDeLlaves");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Reserva", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClienteId")
                        .HasColumnType("int");

                    b.Property<bool>("EstaCancelada")
                        .HasColumnType("bit");

                    b.Property<bool>("EstaElClienteEnHostal")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("FechaCancelacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaEntrada")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaReservacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaSalida")
                        .HasColumnType("datetime2");

                    b.Property<int>("HabitacionId")
                        .HasColumnType("int");

                    b.Property<string>("HabitacionNumero")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Importe")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("MotivoCancelacion")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId");

                    b.HasIndex("HabitacionId");

                    b.HasIndex("HabitacionNumero");

                    b.ToTable("Reservas");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Traza", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Detalles")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<string>("Operacion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistroId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TablaAfectada")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Trazas");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.HabitacionAmaDeLlaves", b =>
                {
                    b.HasOne("HostalIslaAzul.Domain.Entities.AmaDeLlaves", "AmaDeLlaves")
                        .WithMany()
                        .HasForeignKey("AmaDeLlavesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HostalIslaAzul.Domain.Entities.AmaDeLlaves", null)
                        .WithMany("HabitacionesAtendidas")
                        .HasForeignKey("AmaDeLlavesId1");

                    b.HasOne("HostalIslaAzul.Domain.Entities.Habitacion", "Habitacion")
                        .WithMany()
                        .HasForeignKey("HabitacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HostalIslaAzul.Domain.Entities.Habitacion", null)
                        .WithMany("AmasDeLlaves")
                        .HasForeignKey("HabitacionId1");

                    b.Navigation("AmaDeLlaves");

                    b.Navigation("Habitacion");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Reserva", b =>
                {
                    b.HasOne("HostalIslaAzul.Domain.Entities.Cliente", "Cliente")
                        .WithMany("Reservas")
                        .HasForeignKey("ClienteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HostalIslaAzul.Domain.Entities.Habitacion", "Habitacion")
                        .WithMany("Reservas")
                        .HasForeignKey("HabitacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cliente");

                    b.Navigation("Habitacion");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.AmaDeLlaves", b =>
                {
                    b.Navigation("HabitacionesAtendidas");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Cliente", b =>
                {
                    b.Navigation("Reservas");
                });

            modelBuilder.Entity("HostalIslaAzul.Domain.Entities.Habitacion", b =>
                {
                    b.Navigation("AmasDeLlaves");

                    b.Navigation("Reservas");
                });
#pragma warning restore 612, 618
        }
    }
}
