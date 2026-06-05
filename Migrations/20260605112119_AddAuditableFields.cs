using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AircraftMRO.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "WorkOrders",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Alerts",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "WorkOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "WorkOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "MaintenanceRecords",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "MaintenanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "MaintenanceRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "MaintenanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MaintenanceRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "MaintenanceRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "MaintenanceRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Alerts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Alerts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Alerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Alerts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Aircrafts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Aircrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Aircrafts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Aircrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Aircrafts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Aircrafts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_CreatedByUserId",
                table: "WorkOrders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_DeletedByUserId",
                table: "WorkOrders",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_UpdatedByUserId",
                table: "WorkOrders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_CreatedByUserId",
                table: "MaintenanceRecords",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_DeletedByUserId",
                table: "MaintenanceRecords",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_UpdatedByUserId",
                table: "MaintenanceRecords",
                column: "UpdatedByUserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Aircrafts_CreatedByUserId",
                table: "Aircrafts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Aircrafts_DeletedByUserId",
                table: "Aircrafts",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Aircrafts_UpdatedByUserId",
                table: "Aircrafts",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AspNetUsers_CreatedByUserId",
                table: "Aircrafts",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AspNetUsers_DeletedByUserId",
                table: "Aircrafts",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AspNetUsers_UpdatedByUserId",
                table: "Aircrafts",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_DeletedByUserId",
                table: "Alerts",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_UpdatedByUserId",
                table: "Alerts",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_AspNetUsers_CreatedByUserId",
                table: "MaintenanceRecords",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_AspNetUsers_DeletedByUserId",
                table: "MaintenanceRecords",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_AspNetUsers_UpdatedByUserId",
                table: "MaintenanceRecords",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_CreatedByUserId",
                table: "WorkOrders",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_DeletedByUserId",
                table: "WorkOrders",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_UpdatedByUserId",
                table: "WorkOrders",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AspNetUsers_CreatedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AspNetUsers_DeletedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AspNetUsers_UpdatedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_DeletedByUserId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_UpdatedByUserId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_AspNetUsers_CreatedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_AspNetUsers_DeletedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_AspNetUsers_UpdatedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_CreatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_DeletedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_UpdatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_CreatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_DeletedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_UpdatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRecords_CreatedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRecords_DeletedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRecords_UpdatedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_DeletedByUserId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_UpdatedByUserId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Aircrafts_CreatedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropIndex(
                name: "IX_Aircrafts_DeletedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropIndex(
                name: "IX_Aircrafts_UpdatedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Aircrafts");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "WorkOrders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Alerts",
                newName: "CreatedAt");
        }
    }
}
