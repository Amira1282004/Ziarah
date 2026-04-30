using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClinicTablesAndClinicIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: safe if clinic tables were already removed manually.
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Schedules_ClinicId' AND object_id = OBJECT_ID(N'[Ziarah_schema].[Schedules]'))
                    DROP INDEX [IX_Schedules_ClinicId] ON [Ziarah_schema].[Schedules];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Schedules_DoctorClinicDay' AND object_id = OBJECT_ID(N'[Ziarah_schema].[Schedules]'))
                    DROP INDEX [UQ_Schedules_DoctorClinicDay] ON [Ziarah_schema].[Schedules];
                """);

            migrationBuilder.Sql("""
                WHILE EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.parent_object_id = OBJECT_ID(N'[Ziarah_schema].[Schedules]') AND c.name = N'ClinicId')
                BEGIN
                    DECLARE @s sysname;
                    SELECT TOP 1 @s = fk.name
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.parent_object_id = OBJECT_ID(N'[Ziarah_schema].[Schedules]') AND c.name = N'ClinicId';
                    DECLARE @ds nvarchar(max) = N'ALTER TABLE [Ziarah_schema].[Schedules] DROP CONSTRAINT ' + QUOTENAME(@s) + N';';
                    EXEC sp_executesql @ds;
                END
                IF COL_LENGTH(N'[Ziarah_schema].[Schedules]', N'ClinicId') IS NOT NULL
                    ALTER TABLE [Ziarah_schema].[Schedules] DROP COLUMN [ClinicId];
                """);

            migrationBuilder.Sql("""
                WHILE EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.parent_object_id = OBJECT_ID(N'[Ziarah_schema].[Appointments]') AND c.name = N'ClinicId')
                BEGIN
                    DECLARE @a sysname;
                    SELECT TOP 1 @a = fk.name
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.parent_object_id = OBJECT_ID(N'[Ziarah_schema].[Appointments]') AND c.name = N'ClinicId';
                    DECLARE @da nvarchar(max) = N'ALTER TABLE [Ziarah_schema].[Appointments] DROP CONSTRAINT ' + QUOTENAME(@a) + N';';
                    EXEC sp_executesql @da;
                END
                IF COL_LENGTH(N'[Ziarah_schema].[Appointments]', N'ClinicId') IS NOT NULL
                    ALTER TABLE [Ziarah_schema].[Appointments] DROP COLUMN [ClinicId];
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Schedules_DoctorDay' AND object_id = OBJECT_ID(N'[Ziarah_schema].[Schedules]'))
                    CREATE UNIQUE INDEX [UQ_Schedules_DoctorDay] ON [Ziarah_schema].[Schedules]([DoctorId], [DayOfWeek]);
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Ziarah_schema].[DoctorClinics]', N'U') IS NOT NULL
                    DROP TABLE [Ziarah_schema].[DoctorClinics];
                IF OBJECT_ID(N'[Ziarah_schema].[Clinics]', N'U') IS NOT NULL
                    DROP TABLE [Ziarah_schema].[Clinics];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Schedules_DoctorDay",
                schema: "Ziarah_schema",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                schema: "Ziarah_schema",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                schema: "Ziarah_schema",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Clinics",
                schema: "Ziarah_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClosingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OpeningTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clinics__3214EC075555E5BA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clinics_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Ziarah_schema",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DoctorClinics",
                schema: "Ziarah_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: false),
                    ConsultationPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoctorCl__3214EC07053C2733", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorClinics_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Ziarah_schema",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClinicId",
                schema: "Ziarah_schema",
                table: "Schedules",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "UQ_Schedules_DoctorClinicDay",
                schema: "Ziarah_schema",
                table: "Schedules",
                columns: new[] { "DoctorId", "ClinicId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_CreatedBy",
                schema: "Ziarah_schema",
                table: "Clinics",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorClinics_CreatedBy",
                schema: "Ziarah_schema",
                table: "DoctorClinics",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "UQ_DoctorClinics_DoctorClinic",
                schema: "Ziarah_schema",
                table: "DoctorClinics",
                columns: new[] { "DoctorId", "ClinicId" },
                unique: true);
        }
    }
}
