using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class NewDataOnGameAndPlayerEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Lineups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LineupId",
                table: "Games",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_LineupId",
                table: "Games",
                column: "LineupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Lineups_LineupId",
                table: "Games",
                column: "LineupId",
                principalTable: "Lineups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Lineups_LineupId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_LineupId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Lineups");

            migrationBuilder.DropColumn(
                name: "LineupId",
                table: "Games");
        }
    }
}
