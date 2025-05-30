﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartCraft.Core.Tellus.Infrastructure.Context;

#nullable disable

namespace SmartCraft.Core.Tellus.Infrastructure.Migrations
{
    [DbContext(typeof(CompanyContext))]
    [Migration("20250428142218_NameFieldOnCompany")]
    partial class NameFieldOnCompany
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SmartCraft.Core.Tellus.Infrastructure.Models.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("DaimlerToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LastUpdatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("ManToken")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ScaniaClientId")
                        .HasColumnType("text");

                    b.Property<string>("ScaniaSecretKey")
                        .HasColumnType("text");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid");

                    b.Property<string>("VolvoCredentials")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id", "TenantId")
                        .IsUnique();

                    b.ToTable("Companies");
                });
#pragma warning restore 612, 618
        }
    }
}
