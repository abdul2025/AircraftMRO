using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AircraftMRO.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationAddEmailNotificationDiscardAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "AircraftId",
                table: "Notifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailProcessed",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmailNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotificationId = table.Column<int>(type: "integer", nullable: false),
                    RecipientEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailNotifications_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AircraftId",
                table: "Notifications",
                column: "AircraftId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifications_NotificationId",
                table: "EmailNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifications_SentAtUtc",
                table: "EmailNotifications",
                column: "SentAtUtc");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Aircrafts_AircraftId",
                table: "Notifications",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Aircrafts_AircraftId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "EmailNotifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_AircraftId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AircraftId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsEmailProcessed",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Notifications");

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AircraftId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationSent = table.Column<bool>(type: "boolean", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Severity = table.Column<int>(type: "integer", maxLength: 300, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WorkOrderIds = table.Column<List<int>>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Aircrafts_AircraftId",
                        column: x => x.AircraftId,
                        principalTable: "Aircrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alerts_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alerts_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_AircraftId",
                table: "Alerts",
                column: "AircraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DeletedByUserId",
                table: "Alerts",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_UpdatedByUserId",
                table: "Alerts",
                column: "UpdatedByUserId");
        }
    }
}
