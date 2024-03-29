﻿// <auto-generated />
using System;
using GeekShopping.Email.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeekShopping.Email.Migrations
{
    [DbContext(typeof(EmailContext))]
    partial class EmailContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.13");

            modelBuilder.Entity("GeekShopping.Email.Model.EmailLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("sent_date");

                    b.Property<string>("log")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("log");

                    b.HasKey("Id");

                    b.ToTable("email_logs");
                });
#pragma warning restore 612, 618
        }
    }
}
