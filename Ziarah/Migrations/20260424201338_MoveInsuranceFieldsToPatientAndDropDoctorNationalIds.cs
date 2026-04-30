using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class MoveInsuranceFieldsToPatientAndDropDoctorNationalIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasInsurance",
                schema: "Ziarah_schema",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsuranceImage",
                schema: "Ziarah_schema",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Doctors");

            migrationBuilder.AddColumn<bool>(
                name: "HasInsurance",
                schema: "Ziarah_schema",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceImage",
                schema: "Ziarah_schema",
                table: "Patients",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasInsurance",
                schema: "Ziarah_schema",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "InsuranceImage",
                schema: "Ziarah_schema",
                table: "Patients");

            migrationBuilder.AddColumn<bool>(
                name: "HasInsurance",
                schema: "Ziarah_schema",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceImage",
                schema: "Ziarah_schema",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
