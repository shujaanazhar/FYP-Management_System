﻿// <auto-generated />
using FYP_Management_System.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FYP_Management_System.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240320112358_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NexGen.Models.FYP", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SupervisorId")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.HasIndex("SupervisorId");

                    b.ToTable("FYPs");
                });

            modelBuilder.Entity("NexGen.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Batch")
                        .HasColumnType("int");

                    b.Property<float>("CGPA")
                        .HasColumnType("real");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("FYPName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FYP_Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FYPName");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("NexGen.Models.Supervisor", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Supervisors");
                });

            modelBuilder.Entity("NexGen.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NexGen.Models.FYP", b =>
                {
                    b.HasOne("NexGen.Models.Supervisor", "Supervisor")
                        .WithMany("FYP")
                        .HasForeignKey("SupervisorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supervisor");
                });

            modelBuilder.Entity("NexGen.Models.Student", b =>
                {
                    b.HasOne("NexGen.Models.FYP", "FYP")
                        .WithMany("Students")
                        .HasForeignKey("FYPName");

                    b.Navigation("FYP");
                });

            modelBuilder.Entity("NexGen.Models.Supervisor", b =>
                {
                    b.HasOne("NexGen.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("NexGen.Models.FYP", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("NexGen.Models.Supervisor", b =>
                {
                    b.Navigation("FYP");
                });
#pragma warning restore 612, 618
        }
    }
}
