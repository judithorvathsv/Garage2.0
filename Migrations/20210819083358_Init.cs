using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Garage2._0.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VehicleModel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NumberOfWheels = table.Column<int>(type: "int", nullable: false),
                    IsParked = table.Column<bool>(type: "bit", nullable: false),
                    TimeOfArrival = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicle");
        }
    }
}
