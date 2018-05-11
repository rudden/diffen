using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class ChangeOnInvite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Invites",
                newName: "UniqueCode");

            migrationBuilder.AddColumn<string>(
                name: "InviteUsedByUserId",
                table: "Invites",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_InviteUsedByUserId",
                table: "Invites",
                column: "InviteUsedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_AspNetUsers_InviteUsedByUserId",
                table: "Invites",
                column: "InviteUsedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_AspNetUsers_InviteUsedByUserId",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_InviteUsedByUserId",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "InviteUsedByUserId",
                table: "Invites");

            migrationBuilder.RenameColumn(
                name: "UniqueCode",
                table: "Invites",
                newName: "Email");
        }
    }
}
