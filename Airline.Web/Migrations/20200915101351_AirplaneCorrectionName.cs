using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Airline.Web.Migrations
{
    public partial class AirplaneCorrectionName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airplaines_AirplaineId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "Airplaines");

            migrationBuilder.RenameColumn(
                name: "AirplaineId",
                table: "Flights",
                newName: "AirplaneId");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_AirplaineId",
                table: "Flights",
                newName: "IX_Flights_AirplaneId");

            migrationBuilder.CreateTable(
                name: "Airplanes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(maxLength: 50, nullable: false),
                    Model = table.Column<string>(maxLength: 50, nullable: false),
                    EconomySeats = table.Column<int>(nullable: false),
                    BusinessSeats = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airplanes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Airplanes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airplanes_UserId",
                table: "Airplanes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airplanes_AirplaneId",
                table: "Flights",
                column: "AirplaneId",
                principalTable: "Airplanes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airplanes_AirplaneId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "Airplanes");

            migrationBuilder.RenameColumn(
                name: "AirplaneId",
                table: "Flights",
                newName: "AirplaineId");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_AirplaneId",
                table: "Flights",
                newName: "IX_Flights_AirplaineId");

            migrationBuilder.CreateTable(
                name: "Airplaines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(maxLength: 50, nullable: false),
                    BusinessSeats = table.Column<int>(nullable: false),
                    EconomySeats = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Model = table.Column<string>(maxLength: 50, nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airplaines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Airplaines_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airplaines_UserId",
                table: "Airplaines",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airplaines_AirplaineId",
                table: "Flights",
                column: "AirplaineId",
                principalTable: "Airplaines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
