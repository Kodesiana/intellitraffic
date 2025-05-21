using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix_history_fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_analysis_histories_analysis_results_camera_id",
                table: "analysis_histories");

            migrationBuilder.DropIndex(
                name: "ix_analysis_histories_camera_id",
                table: "analysis_histories");

            migrationBuilder.RenameColumn(
                name: "analysis_result_id",
                table: "analysis_histories",
                newName: "result_id");

            migrationBuilder.CreateIndex(
                name: "ix_analysis_histories_camera_id",
                table: "analysis_histories",
                column: "camera_id");

            migrationBuilder.CreateIndex(
                name: "ix_analysis_histories_result_id",
                table: "analysis_histories",
                column: "result_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_analysis_histories_analysis_results_result_id",
                table: "analysis_histories",
                column: "result_id",
                principalTable: "analysis_results",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_analysis_histories_analysis_results_result_id",
                table: "analysis_histories");

            migrationBuilder.DropIndex(
                name: "ix_analysis_histories_camera_id",
                table: "analysis_histories");

            migrationBuilder.DropIndex(
                name: "ix_analysis_histories_result_id",
                table: "analysis_histories");

            migrationBuilder.RenameColumn(
                name: "result_id",
                table: "analysis_histories",
                newName: "analysis_result_id");

            migrationBuilder.CreateIndex(
                name: "ix_analysis_histories_camera_id",
                table: "analysis_histories",
                column: "camera_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_analysis_histories_analysis_results_camera_id",
                table: "analysis_histories",
                column: "camera_id",
                principalTable: "analysis_results",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
