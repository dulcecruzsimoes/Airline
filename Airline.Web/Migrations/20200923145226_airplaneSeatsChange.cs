using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Airline.Web.Migrations
{
    public partial class airplaneSeatsChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_States_StateId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.RenameColumn(
                name: "StateId",
                table: "Flights",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_StateId",
                table: "Flights",
                newName: "IX_Flights_StatusId");

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StatusName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Status_StatusName",
                table: "Status",
                column: "StatusName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Status_StatusId",
                table: "Flights",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Status_StatusId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Flights",
                newName: "StateId");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_StatusId",
                table: "Flights",
                newName: "IX_Flights_StateId");

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StateName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_States_StateName",
                table: "States",
                column: "StateName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_States_StateId",
                table: "Flights",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
