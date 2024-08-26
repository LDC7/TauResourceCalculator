﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TauResourceCalculator.Infrastructure.Data.SQLite;

#nullable disable

namespace TauResourceCalculator.Infrastructure.Data.SQLite.Migrations
{
    [DbContext(typeof(ApplicationSQLiteDbContext))]
    partial class ApplicationSQLiteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Member", b =>
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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Project", b =>
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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.ResourceModifier", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Day")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("MemberId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Resource")
                        .HasColumnType("REAL");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("TeamId");

                    b.ToTable("ResourceModifier");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Sprint", b =>
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

                    b.ToTable("Sprints");

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.SprintEntry", b =>
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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Team", b =>
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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Member", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Team", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.ResourceModifier", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Team", "Team")
                        .WithMany("ResourceModifiers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Sprint", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Project", "Project")
                        .WithMany("Sprints")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.SprintEntry", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Sprint", "Sprint")
                        .WithMany("Entries")
                        .HasForeignKey("SprintId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sprint");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Project", b =>
                {
                    b.Navigation("Sprints");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Sprint", b =>
                {
                    b.Navigation("Entries");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Team", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("ResourceModifiers");
                });
#pragma warning restore 612, 618
        }
    }
}
