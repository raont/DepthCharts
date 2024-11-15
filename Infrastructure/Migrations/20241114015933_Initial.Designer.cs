﻿// <auto-generated />
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(PlayerDepthsContext))]
    [Migration("20241114015933_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Core.Entities.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("GameId");

                    b.ToTable("Game", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("PlayerId", "GameId");

                    b.HasIndex("GameId");

                    b.ToTable("Player", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Position", b =>
                {
                    b.Property<string>("Depth")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Game")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Position1")
                        .HasColumnType("INTEGER")
                        .HasColumnName("Position");

                    b.HasIndex("Game");

                    b.ToTable("Position", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Player", b =>
                {
                    b.HasOne("Core.Entities.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Core.Entities.Position", b =>
                {
                    b.HasOne("Core.Entities.Game", "GameNavigation")
                        .WithMany()
                        .HasForeignKey("Game");

                    b.Navigation("GameNavigation");
                });

            modelBuilder.Entity("Core.Entities.Game", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
