using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LocationCapture.DAL.SqlServer;

namespace LocationCapture.DAL.SqlServer.Migrations
{
    [DbContext(typeof(SqlServerLocationDbContext))]
    partial class SqlServerLocationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<string>("Thumbnail");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("LocationSnapshots");
                });

            modelBuilder.Entity("LocationCapture.Models.LocationSnapshot", b =>
                {
                    b.HasOne("LocationCapture.Models.Location")
                        .WithMany("LocationSnapshots")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
