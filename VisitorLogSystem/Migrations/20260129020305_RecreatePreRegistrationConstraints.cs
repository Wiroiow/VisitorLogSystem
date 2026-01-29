using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLogSystem.Migrations
{
    /// <inheritdoc />
    public partial class RecreatePreRegistrationConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreRegisteredVisitors_room_visits_RoomVisitId",
                table: "PreRegisteredVisitors");

            migrationBuilder.AddForeignKey(
                name: "FK_PreRegisteredVisitors_room_visits_RoomVisitId",
                table: "PreRegisteredVisitors",
                column: "RoomVisitId",
                principalTable: "room_visits",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreRegisteredVisitors_room_visits_RoomVisitId",
                table: "PreRegisteredVisitors");

            migrationBuilder.AddForeignKey(
                name: "FK_PreRegisteredVisitors_room_visits_RoomVisitId",
                table: "PreRegisteredVisitors",
                column: "RoomVisitId",
                principalTable: "room_visits",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
