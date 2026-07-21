using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uam.LabHelpDesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFaultReportStatusLogsAndAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "FaultReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FaultReportStatusLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaultReportId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NewStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultReportStatusLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaultReportStatusLogs_FaultReports_FaultReportId",
                        column: x => x.FaultReportId,
                        principalTable: "FaultReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaultReportStatusLogs_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_AssignedToUserId",
                table: "FaultReports",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReportStatusLogs_ChangedByUserId",
                table: "FaultReportStatusLogs",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReportStatusLogs_FaultReportId",
                table: "FaultReportStatusLogs",
                column: "FaultReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Users_AssignedToUserId",
                table: "FaultReports",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Users_AssignedToUserId",
                table: "FaultReports");

            migrationBuilder.DropTable(
                name: "FaultReportStatusLogs");

            migrationBuilder.DropIndex(
                name: "IX_FaultReports_AssignedToUserId",
                table: "FaultReports");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "FaultReports");
        }
    }
}
