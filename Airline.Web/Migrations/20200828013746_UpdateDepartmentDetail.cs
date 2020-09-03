using Microsoft.EntityFrameworkCore.Migrations;

namespace Airline.Web.Migrations
{
    public partial class UpdateDepartmentDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "DepartmentDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "DepartmentDetails",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
