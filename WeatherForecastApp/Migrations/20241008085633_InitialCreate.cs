using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherForecastApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LockoutEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultCityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WeatherInfos",
                columns: table => new
                {
                    WeatherInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeatherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperature = table.Column<float>(type: "real", nullable: false),
                    MainStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherInfos", x => x.WeatherInfoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WeatherInfos");
        }
    }
}
