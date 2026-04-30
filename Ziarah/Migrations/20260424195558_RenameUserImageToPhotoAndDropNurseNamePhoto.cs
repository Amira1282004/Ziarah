using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserImageToPhotoAndDropNurseNamePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Ziarah_schema].[Nurses]
                SET [UserId] = [CreatedBy]
                WHERE [UserId] IS NULL;
                """);

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "Photo",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                schema: "Ziarah_schema",
                table: "Users",
                newName: "Photo");

            migrationBuilder.Sql("""
                UPDATE [Ziarah_schema].[Users]
                SET [Photo] = NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photo",
                schema: "Ziarah_schema",
                table: "Users",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
