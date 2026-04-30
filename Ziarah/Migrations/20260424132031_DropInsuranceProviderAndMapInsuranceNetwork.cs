using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class DropInsuranceProviderAndMapInsuranceNetwork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @createdBy INT = (SELECT TOP (1) [Id] FROM [Ziarah_schema].[Users] ORDER BY [Id]);
                IF @createdBy IS NULL SET @createdBy = 1;

                DECLARE @insuranceId INT = (
                    SELECT TOP (1) [Id]
                    FROM [Ziarah_schema].[Insurance]
                    WHERE [CompanyName] = N'تأمين اعضاء هيئة التدريس'
                );

                IF @insuranceId IS NULL
                BEGIN
                    INSERT INTO [Ziarah_schema].[Insurance]
                        ([CompanyName], [PolicyNumberFormat], [CoverageDetails], [IsActive], [IsDeleted], [CreatedBy], [CreatedOn])
                    VALUES
                        (N'تأمين اعضاء هيئة التدريس', N'FAC-{000000}', N'شبكة علاجية لاعضاء هيئة التدريس تشمل مستشفيات ومعامل وصيدليات ومراكز اشعة.', 1, 0, @createdBy, GETDATE());

                    SET @insuranceId = CAST(SCOPE_IDENTITY() AS INT);
                END;

                INSERT INTO [Ziarah_schema].[Hospital]
                    ([HospitalName], [Location], [Hotline], [IsDeleted], [CreatedBy], [CreatedOn], [InsuranceId])
                SELECT
                    p.[ProviderName],
                    LEFT(LTRIM(RTRIM(CONCAT(
                        COALESCE(NULLIF(p.[Address], N''), N''),
                        CASE WHEN NULLIF(p.[Area], N'') IS NULL THEN N'' ELSE N' - ' + p.[Area] END,
                        CASE WHEN NULLIF(p.[Governorate], N'') IS NULL THEN N'' ELSE N' - ' + p.[Governorate] END
                    ))), 500),
                    LEFT(COALESCE(NULLIF(LTRIM(RTRIM(p.[Phone])), N''), N'غير متاح'), 50),
                    0,
                    @createdBy,
                    GETDATE(),
                    @insuranceId
                FROM [Ziarah_schema].[InsuranceProviders] p
                WHERE p.[IsDeleted] = 0
                  AND p.[ProviderType] NOT LIKE N'%صيد%'
                  AND p.[ProviderType] NOT LIKE N'%أشعة%'
                  AND p.[ProviderType] NOT LIKE N'%معمل%'
                  AND p.[ProviderType] NOT LIKE N'%تحاليل%';

                INSERT INTO [Ziarah_schema].[Lab]
                    ([LabName], [Phone], [Location], [TakingHomeSample], [InsuranceId], [IsDeleted], [CreatedBy], [CreatedOn])
                SELECT
                    p.[ProviderName],
                    LEFT(COALESCE(NULLIF(LTRIM(RTRIM(p.[Phone])), N''), N'غير متاح'), 50),
                    LEFT(LTRIM(RTRIM(CONCAT(
                        COALESCE(NULLIF(p.[Address], N''), N''),
                        CASE WHEN NULLIF(p.[Area], N'') IS NULL THEN N'' ELSE N' - ' + p.[Area] END,
                        CASE WHEN NULLIF(p.[Governorate], N'') IS NULL THEN N'' ELSE N' - ' + p.[Governorate] END
                    ))), 500),
                    0,
                    @insuranceId,
                    0,
                    @createdBy,
                    GETDATE()
                FROM [Ziarah_schema].[InsuranceProviders] p
                WHERE p.[IsDeleted] = 0
                  AND (p.[ProviderType] LIKE N'%معمل%' OR p.[ProviderType] LIKE N'%تحاليل%');

                INSERT INTO [Ziarah_schema].[Pharmacy]
                    ([PharmacyName], [Phone], [Location], [Open24Hours], [DeliveryAvailable], [IsDeleted], [CreatedBy], [CreatedOn], [InsuranceId])
                SELECT
                    p.[ProviderName],
                    LEFT(COALESCE(NULLIF(LTRIM(RTRIM(p.[Phone])), N''), N'غير متاح'), 50),
                    LEFT(LTRIM(RTRIM(CONCAT(
                        COALESCE(NULLIF(p.[Address], N''), N''),
                        CASE WHEN NULLIF(p.[Area], N'') IS NULL THEN N'' ELSE N' - ' + p.[Area] END,
                        CASE WHEN NULLIF(p.[Governorate], N'') IS NULL THEN N'' ELSE N' - ' + p.[Governorate] END
                    ))), 500),
                    0,
                    0,
                    0,
                    @createdBy,
                    GETDATE(),
                    @insuranceId
                FROM [Ziarah_schema].[InsuranceProviders] p
                WHERE p.[IsDeleted] = 0
                  AND p.[ProviderType] LIKE N'%صيد%';

                INSERT INTO [Ziarah_schema].[Radiology]
                    ([NameRadiology], [Phone], [Location], [Types], [IsDeleted], [CreatedBy], [CreatedOn], [InsuranceId])
                SELECT
                    p.[ProviderName],
                    LEFT(COALESCE(NULLIF(LTRIM(RTRIM(p.[Phone])), N''), N'غير متاح'), 50),
                    LEFT(LTRIM(RTRIM(CONCAT(
                        COALESCE(NULLIF(p.[Address], N''), N''),
                        CASE WHEN NULLIF(p.[Area], N'') IS NULL THEN N'' ELSE N' - ' + p.[Area] END,
                        CASE WHEN NULLIF(p.[Governorate], N'') IS NULL THEN N'' ELSE N' - ' + p.[Governorate] END
                    ))), 500),
                    NULLIF(LTRIM(RTRIM(p.[ProviderType])), N''),
                    0,
                    @createdBy,
                    GETDATE(),
                    @insuranceId
                FROM [Ziarah_schema].[InsuranceProviders] p
                WHERE p.[IsDeleted] = 0
                  AND p.[ProviderType] LIKE N'%أشعة%';
                """);

            migrationBuilder.DropTable(
                name: "InsuranceProviders",
                schema: "Ziarah_schema");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsuranceProviders",
                schema: "Ziarah_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Governorate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceProviders", x => x.Id);
                });
        }
    }
}
