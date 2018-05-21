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

            migrationBuilder.CreateTable(
                name: "GameToLineup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(nullable: false),
                    LineupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameToLineup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameToLineup_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameToLineup_Lineups_LineupId",
                        column: x => x.LineupId,
                        principalTable: "Lineups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LineupId",
                table: "Games",
                column: "LineupId");

            migrationBuilder.CreateIndex(
                name: "IX_GameToLineup_GameId",
                table: "GameToLineup",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameToLineup_LineupId",
                table: "GameToLineup",
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

            migrationBuilder.DropTable(
                name: "GameToLineup");

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
