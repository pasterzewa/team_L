﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VaccinationSystem.Data;

namespace VaccinationSystem.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VaccinationSystem.Models.Admin", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("dateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("firstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pesel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Appointment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Doctorid")
                        .HasColumnType("int");

                    b.Property<int?>("Doctorid1")
                        .HasColumnType("int");

                    b.Property<int?>("Patientid")
                        .HasColumnType("int");

                    b.Property<int?>("Patientid1")
                        .HasColumnType("int");

                    b.Property<bool>("completed")
                        .HasColumnType("bit");

                    b.Property<int?>("patientid")
                        .HasColumnType("int");

                    b.Property<int?>("timeSlotid")
                        .HasColumnType("int");

                    b.Property<string>("vaccineBatchNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("vaccineid")
                        .HasColumnType("int");

                    b.Property<int>("whichDose")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("Doctorid");

                    b.HasIndex("Doctorid1");

                    b.HasIndex("Patientid");

                    b.HasIndex("Patientid1");

                    b.HasIndex("patientid");

                    b.HasIndex("timeSlotid");

                    b.HasIndex("vaccineid");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Certificate", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Patientid")
                        .HasColumnType("int");

                    b.Property<string>("url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("Patientid");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Doctor", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("dateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("firstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("patientAccountid")
                        .HasColumnType("int");

                    b.Property<string>("pesel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("vaccinationCenterid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("patientAccountid");

                    b.HasIndex("vaccinationCenterid");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Patient", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("dateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("firstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pesel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("VaccinationSystem.Models.TimeSlot", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<int?>("doctorid")
                        .HasColumnType("int");

                    b.Property<DateTime>("from")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isFree")
                        .HasColumnType("bit");

                    b.Property<DateTime>("to")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.HasIndex("doctorid");

                    b.ToTable("TimeSlot");
                });

            modelBuilder.Entity("VaccinationSystem.Models.VaccinationCenter", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("city")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("VaccinationCenters");
                });

            modelBuilder.Entity("VaccinationSystem.Models.VaccinationCount", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("count")
                        .HasColumnType("int");

                    b.Property<int?>("patientid")
                        .HasColumnType("int");

                    b.Property<int>("virus")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("patientid");

                    b.ToTable("VaccinationCounts");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Vaccine", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("VaccinationCenterid")
                        .HasColumnType("int");

                    b.Property<string>("company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("maxDaysBetweenDoses")
                        .HasColumnType("int");

                    b.Property<int>("maxPatientAge")
                        .HasColumnType("int");

                    b.Property<int>("minDaysBetweenDoses")
                        .HasColumnType("int");

                    b.Property<int>("minPatientAge")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("numberOfDoses")
                        .HasColumnType("int");

                    b.Property<bool>("used")
                        .HasColumnType("bit");

                    b.Property<int>("virus")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("VaccinationCenterid");

                    b.ToTable("Vaccines");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Appointment", b =>
                {
                    b.HasOne("VaccinationSystem.Models.Doctor", null)
                        .WithMany("futureVaccinations")
                        .HasForeignKey("Doctorid");

                    b.HasOne("VaccinationSystem.Models.Doctor", null)
                        .WithMany("vaccinationsArchive")
                        .HasForeignKey("Doctorid1");

                    b.HasOne("VaccinationSystem.Models.Patient", null)
                        .WithMany("vaccinationHistory")
                        .HasForeignKey("Patientid");

                    b.HasOne("VaccinationSystem.Models.Patient", null)
                        .WithMany("futureVaccinations")
                        .HasForeignKey("Patientid1");

                    b.HasOne("VaccinationSystem.Models.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("patientid");

                    b.HasOne("VaccinationSystem.Models.TimeSlot", "timeSlot")
                        .WithMany()
                        .HasForeignKey("timeSlotid");

                    b.HasOne("VaccinationSystem.Models.Vaccine", "vaccine")
                        .WithMany()
                        .HasForeignKey("vaccineid");

                    b.Navigation("patient");

                    b.Navigation("timeSlot");

                    b.Navigation("vaccine");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Certificate", b =>
                {
                    b.HasOne("VaccinationSystem.Models.Patient", null)
                        .WithMany("certificates")
                        .HasForeignKey("Patientid");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Doctor", b =>
                {
                    b.HasOne("VaccinationSystem.Models.Patient", "patientAccount")
                        .WithMany()
                        .HasForeignKey("patientAccountid");

                    b.HasOne("VaccinationSystem.Models.VaccinationCenter", "vaccinationCenter")
                        .WithMany("doctors")
                        .HasForeignKey("vaccinationCenterid");

                    b.Navigation("patientAccount");

                    b.Navigation("vaccinationCenter");
                });

            modelBuilder.Entity("VaccinationSystem.Models.TimeSlot", b =>
                {
                    b.HasOne("VaccinationSystem.Models.Doctor", "doctor")
                        .WithMany()
                        .HasForeignKey("doctorid");

                    b.Navigation("doctor");
                });

            modelBuilder.Entity("VaccinationSystem.Models.VaccinationCount", b =>
                {
                    b.HasOne("VaccinationSystem.Models.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("patientid");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Vaccine", b =>
                {
                    b.HasOne("VaccinationSystem.Models.VaccinationCenter", null)
                        .WithMany("availableVaccines")
                        .HasForeignKey("VaccinationCenterid");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Doctor", b =>
                {
                    b.Navigation("futureVaccinations");

                    b.Navigation("vaccinationsArchive");
                });

            modelBuilder.Entity("VaccinationSystem.Models.Patient", b =>
                {
                    b.Navigation("certificates");

                    b.Navigation("futureVaccinations");

                    b.Navigation("vaccinationHistory");
                });

            modelBuilder.Entity("VaccinationSystem.Models.VaccinationCenter", b =>
                {
                    b.Navigation("availableVaccines");

                    b.Navigation("doctors");
                });
#pragma warning restore 612, 618
        }
    }
}
