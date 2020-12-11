﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RH.EntityFramework.Shared.DbContexts;

namespace RH.EntityFramework.Shared.Migrations
{
    [DbContext(typeof(WeatherDbContext))]
    [Migration("20201211113833_InitialMySql")]
    partial class InitialMySql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Dimension", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<short>("X")
                        .HasColumnType("smallint");

                    b.Property<short>("Y")
                        .HasColumnType("smallint");

                    b.Property<short>("Zoom")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Dimensions");
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Ecmwf", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("DataString")
                        .HasColumnType("text");

                    b.Property<int>("DimensionId")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime");

                    b.Property<int>("WindyTimeId")
                        .HasColumnType("int");

                    b.Property<double>("X")
                        .HasColumnType("double");

                    b.Property<double>("Y")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("DimensionId");

                    b.HasIndex("WindyTimeId");

                    b.ToTable("Ecmwfs");
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Forecast", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("DimensionId")
                        .HasColumnType("int");

                    b.Property<string>("ECMWFContent")
                        .HasColumnType("text");

                    b.Property<string>("GFSContent")
                        .HasColumnType("text");

                    b.Property<long>("Start")
                        .HasColumnType("bigint");

                    b.Property<short>("Step")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("DimensionId");

                    b.ToTable("Forecasts");
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Gfs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("DataString")
                        .HasColumnType("text");

                    b.Property<int>("DimensionId")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime");

                    b.Property<int>("WindyTimeId")
                        .HasColumnType("int");

                    b.Property<double>("X")
                        .HasColumnType("double");

                    b.Property<double>("Y")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("DimensionId");

                    b.HasIndex("WindyTimeId");

                    b.ToTable("Gfses");
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Label", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("DimensionId")
                        .HasColumnType("int");

                    b.Property<int>("ExtraField1")
                        .HasColumnType("int");

                    b.Property<int>("ExtraField2")
                        .HasColumnType("int");

                    b.Property<string>("FullText")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("O")
                        .HasColumnType("text");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.Property<double>("X")
                        .HasColumnType("double");

                    b.Property<double>("Y")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("DimensionId");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.WindyTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("Start")
                        .HasColumnType("bigint");

                    b.Property<short>("Step")
                        .HasColumnType("smallint");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WindyTimes");
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Ecmwf", b =>
                {
                    b.HasOne("RH.EntityFramework.Shared.Entities.Dimension", "Dimension")
                        .WithMany()
                        .HasForeignKey("DimensionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RH.EntityFramework.Shared.Entities.WindyTime", "WindyTime")
                        .WithMany()
                        .HasForeignKey("WindyTimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Forecast", b =>
                {
                    b.HasOne("RH.EntityFramework.Shared.Entities.Dimension", "Dimension")
                        .WithMany("Forecasts")
                        .HasForeignKey("DimensionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Gfs", b =>
                {
                    b.HasOne("RH.EntityFramework.Shared.Entities.Dimension", "Dimension")
                        .WithMany()
                        .HasForeignKey("DimensionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RH.EntityFramework.Shared.Entities.WindyTime", "WindyTime")
                        .WithMany()
                        .HasForeignKey("WindyTimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RH.EntityFramework.Shared.Entities.Label", b =>
                {
                    b.HasOne("RH.EntityFramework.Shared.Entities.Dimension", "Dimension")
                        .WithMany("Labels")
                        .HasForeignKey("DimensionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}