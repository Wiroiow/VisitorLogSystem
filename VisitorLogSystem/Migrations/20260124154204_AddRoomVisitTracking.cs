using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLogSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomVisitTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "room_visits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    visitor_id = table.Column<int>(type: "int", nullable: false),
                    room_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entered_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_visits", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_visits_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_visits_entered_at",
                table: "room_visits",
                column: "entered_at");

            migrationBuilder.CreateIndex(
                name: "IX_room_visits_room_name",
                table: "room_visits",
                column: "room_name");

            migrationBuilder.CreateIndex(
                name: "IX_room_visits_visitor_id",
                table: "room_visits",
                column: "visitor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_visits");

            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");
        }
    }
}
