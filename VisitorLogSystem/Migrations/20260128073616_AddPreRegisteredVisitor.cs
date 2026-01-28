using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLogSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPreRegisteredVisitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_room_visits_visitors_visitor_id",
                table: "room_visits");

            migrationBuilder.DropIndex(
                name: "IX_visitors_time_in",
                table: "visitors");

            migrationBuilder.DropIndex(
                name: "IX_room_visits_entered_at",
                table: "room_visits");

            migrationBuilder.DropIndex(
                name: "IX_room_visits_room_name",
                table: "room_visits");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "visitors",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "visitors",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "room_visits",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "PreRegisteredVisitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedVisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HostUserId = table.Column<int>(type: "int", nullable: false),
                    IsCheckedIn = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedInByUserId = table.Column<int>(type: "int", nullable: true),
                    CheckedInAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoomVisitId = table.Column<int>(type: "int", nullable: true),
                    RoomName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegisteredVisitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreRegisteredVisitors_room_visits_RoomVisitId",
                        column: x => x.RoomVisitId,
                        principalTable: "room_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreRegisteredVisitors_users_CheckedInByUserId",
                        column: x => x.CheckedInByUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreRegisteredVisitors_users_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_visitors_full_name",
                table: "visitors",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegisteredVisitors_CheckedInByUserId",
                table: "PreRegisteredVisitors",
                column: "CheckedInByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegisteredVisitors_ExpectedVisitDate",
                table: "PreRegisteredVisitors",
                column: "ExpectedVisitDate");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegisteredVisitors_FullName",
                table: "PreRegisteredVisitors",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegisteredVisitors_HostUserId",
                table: "PreRegisteredVisitors",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegisteredVisitors_IsCheckedIn",
                table: "PreRegisteredVisitors",
                column: "IsCheckedIn");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegisteredVisitors_RoomVisitId",
                table: "PreRegisteredVisitors",
                column: "RoomVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_room_visits_visitors_visitor_id",
                table: "room_visits",
                column: "visitor_id",
                principalTable: "visitors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_room_visits_visitors_visitor_id",
                table: "room_visits");

            migrationBuilder.DropTable(
                name: "PreRegisteredVisitors");

            migrationBuilder.DropIndex(
                name: "IX_visitors_full_name",
                table: "visitors");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "visitors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "visitors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "room_visits",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_visitors_time_in",
                table: "visitors",
                column: "time_in");

            migrationBuilder.CreateIndex(
                name: "IX_room_visits_entered_at",
                table: "room_visits",
                column: "entered_at");

            migrationBuilder.CreateIndex(
                name: "IX_room_visits_room_name",
                table: "room_visits",
                column: "room_name");

            migrationBuilder.AddForeignKey(
                name: "FK_room_visits_visitors_visitor_id",
                table: "room_visits",
                column: "visitor_id",
                principalTable: "visitors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
