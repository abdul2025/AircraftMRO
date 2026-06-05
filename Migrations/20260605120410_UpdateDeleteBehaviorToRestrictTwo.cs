using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AircraftMRO.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorToRestrictTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Aircrafts_AircraftId",
                table: "WorkOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Aircrafts_AircraftId",
                table: "WorkOrders",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Aircrafts_AircraftId",
                table: "WorkOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Aircrafts_AircraftId",
                table: "WorkOrders",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
