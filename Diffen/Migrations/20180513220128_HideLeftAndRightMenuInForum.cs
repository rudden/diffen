using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class HideLeftAndRightMenuInForum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HideLeftMenu",
                table: "UserFilters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideRightMenu",
                table: "UserFilters",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideLeftMenu",
                table: "UserFilters");

            migrationBuilder.DropColumn(
                name: "HideRightMenu",
                table: "UserFilters");
        }
    }
}
