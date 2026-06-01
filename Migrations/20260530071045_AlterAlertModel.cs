using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AircraftMRO.Migrations
{
    /// <inheritdoc />
    public partial class AlterAlertModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsResolved",
                table: "Alerts",
                newName: "NotificationSent");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Alerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "Alerts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "NotificationSent",
                table: "Alerts",
                newName: "IsResolved");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alerts",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
