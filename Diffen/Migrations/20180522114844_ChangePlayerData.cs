using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class ChangePlayerData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadSides",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "GoodSides",
                table: "Players",
                newName: "About");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "About",
                table: "Players",
                newName: "GoodSides");

            migrationBuilder.AddColumn<string>(
                name: "BadSides",
                table: "Players",
                nullable: true);
        }
    }
}
