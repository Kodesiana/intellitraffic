using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial_schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cameras",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    given_name = table.Column<string>(type: "TEXT", nullable: false),
                    overlay_name = table.Column<string>(type: "TEXT", nullable: false),
                    latitude = table.Column<double>(type: "REAL", nullable: false),
                    longitude = table.Column<double>(type: "REAL", nullable: false),
                    hls_stream_name = table.Column<string>(type: "TEXT", nullable: false),
                    google_maps_url = table.Column<string>(type: "TEXT", nullable: false),
                    video_resolution = table.Column<string>(type: "TEXT", nullable: false),
                    video_frame_rate = table.Column<int>(type: "INTEGER", nullable: false),
                    video_format = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cameras", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "analysis_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    total_vehicles = table.Column<int>(type: "INTEGER", nullable: false),
                    has_crowding = table.Column<bool>(type: "INTEGER", nullable: false),
                    has_accident = table.Column<bool>(type: "INTEGER", nullable: false),
                    traffic_density = table.Column<int>(type: "INTEGER", nullable: false),
                    summary = table.Column<string>(type: "TEXT", nullable: false),
                    snapshot_url = table.Column<string>(type: "TEXT", nullable: false),
                    camera_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_analysis_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_analysis_results_cameras_camera_id",
                        column: x => x.camera_id,
                        principalTable: "cameras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "analysis_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    is_camera_online = table.Column<bool>(type: "INTEGER", nullable: false),
                    response = table.Column<string>(type: "TEXT", nullable: true),
                    input_tokens = table.Column<int>(type: "INTEGER", nullable: false),
                    output_tokens = table.Column<int>(type: "INTEGER", nullable: false),
                    camera_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    analysis_result_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_analysis_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_analysis_histories_analysis_results_camera_id",
                        column: x => x.camera_id,
                        principalTable: "analysis_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_analysis_histories_cameras_camera_id",
                        column: x => x.camera_id,
                        principalTable: "cameras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_analysis_histories_camera_id",
                table: "analysis_histories",
                column: "camera_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_analysis_results_camera_id",
                table: "analysis_results",
                column: "camera_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis_histories");

            migrationBuilder.DropTable(
                name: "analysis_results");

            migrationBuilder.DropTable(
                name: "cameras");
        }
    }
}
