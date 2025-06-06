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
            modelBuilder.HasAnnotation("ProductVersion", "8.0.12");

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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("MemberId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<double>("Resource")
                        .HasColumnType("REAL");

                    b.Property<string>("ResourceType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Participant");

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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.SprintResourceModifier", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("End")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParticipantId")
                        .HasColumnType("TEXT");

                    b.Property<double>("Resource")
                        .HasColumnType("REAL");

                    b.Property<Guid>("SprintId")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("SprintId");

                    b.ToTable("SprintResourceModifier");

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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.TeamResourceModifier", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Day")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("MemberId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Resource")
                        .HasColumnType("REAL");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WeekIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamResourceModifier");

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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Participant", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Member", null)
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Project", "Project")
                        .WithMany("Participants")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
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

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.SprintResourceModifier", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Participant", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId");

                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Sprint", "Sprint")
                        .WithMany("ResourceModifiers")
                        .HasForeignKey("SprintId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");

                    b.Navigation("Sprint");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.TeamResourceModifier", b =>
                {
                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("TauResourceCalculator.Domain.ResourceCalculator.Models.Team", "Team")
                        .WithMany("ResourceModifiers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Project", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("Sprints");
                });

            modelBuilder.Entity("TauResourceCalculator.Domain.ResourceCalculator.Models.Sprint", b =>
                {
                    b.Navigation("ResourceModifiers");
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
