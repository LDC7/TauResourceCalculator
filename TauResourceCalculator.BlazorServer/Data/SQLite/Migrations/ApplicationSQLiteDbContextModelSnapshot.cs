﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TauResourceCalculator.BlazorServer.Data.SQLite;

#nullable disable

namespace TauResourceCalculator.BlazorServer.Data.SQLite.Migrations
{
    [DbContext(typeof(ApplicationSQLiteDbContext))]
    partial class ApplicationSQLiteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Resource")
                        .HasColumnType("REAL");

                    b.Property<string>("ResourceType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Member");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Projects");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Sprint", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("End")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Sprint");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.SprintEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Resource")
                        .HasColumnType("REAL");

                    b.Property<string>("ResourceType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SprintId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SprintId");

                    b.ToTable("SprintEntry");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Teams");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Member", b =>
                {
                    b.HasOne("TauResourceCalculator.BlazorServer.Models.Team", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Sprint", b =>
                {
                    b.HasOne("TauResourceCalculator.BlazorServer.Models.Project", "Project")
                        .WithMany("Sprints")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.SprintEntry", b =>
                {
                    b.HasOne("TauResourceCalculator.BlazorServer.Models.Sprint", "Sprint")
                        .WithMany("Entries")
                        .HasForeignKey("SprintId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sprint");
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Team", b =>
                {
                    b.OwnsMany("TauResourceCalculator.BlazorServer.Models.DayOfWeekResourceSubstraction", "ResourceSubstractionsPerDay", b1 =>
                        {
                            b1.Property<Guid>("TeamId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Day")
                                .HasColumnType("TEXT");

                            b1.Property<double>("Resource")
                                .HasColumnType("REAL");

                            b1.HasKey("TeamId", "Day");

                            b1.ToTable("DayOfWeekResourceSubstraction");

                            b1.WithOwner()
                                .HasForeignKey("TeamId");
                        });

                    b.Navigation("ResourceSubstractionsPerDay");
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Project", b =>
                {
                    b.Navigation("Sprints");
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Sprint", b =>
                {
                    b.Navigation("Entries");
                });

            modelBuilder.Entity("TauResourceCalculator.BlazorServer.Models.Team", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
