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
                name: "IX_GameToLineup_GameId",
                table: "GameToLineup",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameToLineup_LineupId",
                table: "GameToLineup",
                column: "LineupId");
        }
    }
}
