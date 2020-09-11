using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Airline.Web.Migrations
{
    public partial class dataContextChangeTypeDataDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "DepartmentDetails",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "DepartmentDetails",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
