using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class materialize_latest_analysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "latest_analyses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    camera_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    result_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    last_update = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_latest_analyses", x => x.id);
                    table.ForeignKey(
                        name: "fk_latest_analyses_analysis_results_result_id",
                        column: x => x.result_id,
                        principalTable: "analysis_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_latest_analyses_cameras_camera_id",
                        column: x => x.camera_id,
                        principalTable: "cameras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_latest_analyses_camera_id",
                table: "latest_analyses",
                column: "camera_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_latest_analyses_result_id",
                table: "latest_analyses",
                column: "result_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "latest_analyses");
        }
    }
}
