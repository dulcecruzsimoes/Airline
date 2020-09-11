using Microsoft.EntityFrameworkCore.Migrations;

namespace Airline.Web.Migrations
{
    public partial class formatDateInDepartmentDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SocialSecurityNumber",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TaxNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Cities");

            migrationBuilder.AlterColumn<string>(
                name: "TaxNumber",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SocialSecurityNumber",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SocialSecurityNumber",
                table: "AspNetUsers",
                column: "SocialSecurityNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TaxNumber",
                table: "AspNetUsers",
                column: "TaxNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SocialSecurityNumber",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TaxNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PaisId",
                table: "Cities",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaxNumber",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SocialSecurityNumber",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SocialSecurityNumber",
                table: "AspNetUsers",
                column: "SocialSecurityNumber",
                unique: true,
                filter: "[SocialSecurityNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TaxNumber",
                table: "AspNetUsers",
                column: "TaxNumber",
                unique: true,
                filter: "[TaxNumber] IS NOT NULL");
        }
    }
}
