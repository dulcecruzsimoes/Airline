using Microsoft.EntityFrameworkCore.Migrations;

namespace Airline.Web.Migrations
{
    public partial class bugCities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId1",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId1",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CountryId1",
                table: "Cities");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Cities",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaisId",
                table: "Cities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Cities");

            migrationBuilder.AlterColumn<string>(
                name: "CountryId",
                table: "Cities",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId1",
                table: "Cities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId1",
                table: "Cities",
                column: "CountryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId1",
                table: "Cities",
                column: "CountryId1",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
