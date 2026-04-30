using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiredIdentityAndInsuranceDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InsuranceCardBackImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InsuranceCardFrontImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalLicenseImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InsuranceCardBackImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InsuranceCardFrontImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalLicenseImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsuranceCardBackImage",
                schema: "Ziarah_schema",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "InsuranceCardFrontImage",
                schema: "Ziarah_schema",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ProfessionalLicenseImage",
                schema: "Ziarah_schema",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "InsuranceCardBackImage",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "InsuranceCardFrontImage",
                schema: "Ziarah_schema",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "ProfessionalLicenseImage",
                schema: "Ziarah_schema",
                table: "Nurses");
        }
    }
}
