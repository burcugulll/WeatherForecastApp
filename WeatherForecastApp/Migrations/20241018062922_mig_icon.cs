using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherForecastApp.Migrations
{
    /// <inheritdoc />
    public partial class mig_icon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "WeatherInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "WeatherInfos");
        }
    }
}
