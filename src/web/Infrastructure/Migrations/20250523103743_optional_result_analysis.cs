using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class optional_result_analysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_latest_analyses_analysis_results_result_id",
                table: "latest_analyses");

            migrationBuilder.AlterColumn<Guid>(
                name: "result_id",
                table: "latest_analyses",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "fk_latest_analyses_analysis_results_result_id",
                table: "latest_analyses",
                column: "result_id",
                principalTable: "analysis_results",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_latest_analyses_analysis_results_result_id",
                table: "latest_analyses");

            migrationBuilder.AlterColumn<Guid>(
                name: "result_id",
                table: "latest_analyses",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_latest_analyses_analysis_results_result_id",
                table: "latest_analyses",
                column: "result_id",
                principalTable: "analysis_results",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
