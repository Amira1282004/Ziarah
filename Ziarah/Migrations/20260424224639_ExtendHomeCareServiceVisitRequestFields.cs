using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class ExtendHomeCareServiceVisitRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalCondition",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderType",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedVisitAt",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterEmail",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterFullName",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequesterPhone",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "MedicalCondition",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "RequestedVisitAt",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "RequesterEmail",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "RequesterFullName",
                schema: "Ziarah_schema",
                table: "HomeCareServices");

            migrationBuilder.DropColumn(
                name: "RequesterPhone",
                schema: "Ziarah_schema",
                table: "HomeCareServices");
        }
    }
}
