using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AircraftMRO.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAlertWorkOrderIdss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "Alerts");

            migrationBuilder.AddColumn<List<int>>(
                name: "WorkOrderIds",
                table: "Alerts",
                type: "integer[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkOrderIds",
                table: "Alerts");

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "Alerts",
                type: "integer",
                nullable: true);
        }
    }
}
