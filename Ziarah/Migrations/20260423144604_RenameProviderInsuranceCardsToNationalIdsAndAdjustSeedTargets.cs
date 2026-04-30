using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class RenameProviderInsuranceCardsToNationalIdsAndAdjustSeedTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InsuranceCardFrontImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                newName: "NationalIdFrontImage");

            migrationBuilder.RenameColumn(
                name: "InsuranceCardBackImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                newName: "NationalIdBackImage");

            migrationBuilder.RenameColumn(
                name: "InsuranceCardFrontImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                newName: "NationalIdFrontImage");

            migrationBuilder.RenameColumn(
                name: "InsuranceCardBackImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                newName: "NationalIdBackImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                newName: "InsuranceCardFrontImage");

            migrationBuilder.RenameColumn(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "Nurses",
                newName: "InsuranceCardBackImage");

            migrationBuilder.RenameColumn(
                name: "NationalIdFrontImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                newName: "InsuranceCardFrontImage");

            migrationBuilder.RenameColumn(
                name: "NationalIdBackImage",
                schema: "Ziarah_schema",
                table: "Doctors",
                newName: "InsuranceCardBackImage");
        }
    }
}
