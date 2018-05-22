using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class MoreDataOnPlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BadSides",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDay",
                table: "Players",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractUntil",
                table: "Players",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GoodSides",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeightInCentimeters",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredFoot",
                table: "Players",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadSides",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BirthDay",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ContractUntil",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GoodSides",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HeightInCentimeters",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PreferredFoot",
                table: "Players");
        }
    }
}
