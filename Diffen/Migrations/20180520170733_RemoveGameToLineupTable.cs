using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class RemoveGameToLineupTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameToLineup");

            migrationBuilder.RenameColumn(
                name: "HappenedInMinuteOfGame",
                table: "PlayerEvents",
                newName: "InMinuteOfGame");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InMinuteOfGame",
                table: "PlayerEvents",
                newName: "HappenedInMinuteOfGame");
        }
    }
}
