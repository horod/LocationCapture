using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LocationCapture.DAL.Sqlite.Migrations
{
    public partial class v1SQLiteMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Altitude = table.Column<double>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    PictureFileName = table.Column<string>(nullable: true)//,
                    //PictureFolderPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationSnapshots_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationSnapshots_LocationId",
                table: "LocationSnapshots",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationSnapshots");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
