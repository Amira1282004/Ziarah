using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ziarah.Migrations
{
    /// <inheritdoc />
    public partial class DropHomeCareServiceHospitalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                WHILE EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.parent_object_id = OBJECT_ID(N'[Ziarah_schema].[HomeCareServices]') AND c.name = N'HospitalId')
                BEGIN
                    DECLARE @n sysname;
                    SELECT TOP 1 @n = fk.name
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE fk.parent_object_id = OBJECT_ID(N'[Ziarah_schema].[HomeCareServices]') AND c.name = N'HospitalId';
                    DECLARE @d nvarchar(max) = N'ALTER TABLE [Ziarah_schema].[HomeCareServices] DROP CONSTRAINT ' + QUOTENAME(@n) + N';';
                    EXEC sp_executesql @d;
                END
                """);

            migrationBuilder.DropColumn(
                name: "HospitalId",
                schema: "Ziarah_schema",
                table: "HomeCareServices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                schema: "Ziarah_schema",
                table: "HomeCareServices",
                type: "int",
                nullable: true);
        }
    }
}
