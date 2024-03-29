﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LocationCapture.DAL.Sqlite2;

namespace LocationCapture.DAL.Sqlite.Migrations
{
    [DbContext(typeof(SqliteLocationDbContext))]
    [Migration("20170723175542_v1SQLiteMigration")]
    partial class v1SQLiteMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("LocationCapture.Models.Location", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Name");

                b.HasKey("Id");

                b.ToTable("Locations");
            });

            modelBuilder.Entity("LocationCapture.Models.LocationSnapshot", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<double>("Altitude");

                b.Property<DateTime>("DateCreated");

                b.Property<double>("Latitude");

                b.Property<int>("LocationId");

                b.Property<double>("Longitude");

                b.Property<string>("PictureFileName");

                b.Property<string>("PictureFolderPath");

                b.HasKey("Id");

                b.HasIndex("LocationId");

                b.ToTable("LocationSnapshots");
            });

            modelBuilder.Entity("LocationCapture.Models.LocationSnapshot", b =>
            {
                b.HasOne("LocationCapture.Models.Location")
                    .WithMany("Snapshots")
                    .HasForeignKey("LocationId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}