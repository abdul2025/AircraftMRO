using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AircraftMRO.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Aircrafts_AircraftId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_WorkOrders_WorkOrderId",
                table: "MaintenanceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Aircrafts_AircraftId",
                table: "Alerts",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_WorkOrders_WorkOrderId",
                table: "MaintenanceRecords",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Aircrafts_AircraftId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_WorkOrders_WorkOrderId",
                table: "MaintenanceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Aircrafts_AircraftId",
                table: "Alerts",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_WorkOrders_WorkOrderId",
                table: "MaintenanceRecords",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
