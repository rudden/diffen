using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class AddGameResultGuesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameResultGuesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    GuessedByUserId = table.Column<string>(nullable: true),
                    NumberOfGoalsScoredByDif = table.Column<int>(nullable: false),
                    NumberOfGoalsScoredByOpponent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResultGuesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameResultGuesses_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameResultGuesses_AspNetUsers_GuessedByUserId",
                        column: x => x.GuessedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameResultGuesses_GameId",
                table: "GameResultGuesses",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResultGuesses_GuessedByUserId",
                table: "GameResultGuesses",
                column: "GuessedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameResultGuesses");
        }
    }
}
