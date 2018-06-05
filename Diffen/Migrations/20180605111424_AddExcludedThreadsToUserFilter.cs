using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Diffen.Migrations
{
    public partial class AddExcludedThreadsToUserFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExcludedThreadIds",
                table: "UserFilters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludedThreadIds",
                table: "UserFilters");
        }
    }
}
