using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class AddInsuranceFacilityImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Radiology",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Pharmacy",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Lab",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Hospital",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Radiology");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Pharmacy");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Lab");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Hospital");
        }
    }
}
