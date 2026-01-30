using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLogSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                });

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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_visits", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_visits_Visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "Visitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    CheckedInAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                        onDelete: ReferentialAction.SetNull);
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

            migrationBuilder.CreateIndex(
                name: "IX_room_visits_visitor_id",
                table: "room_visits",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_FullName",
                table: "Visitors",
                column: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreRegisteredVisitors");

            migrationBuilder.DropTable(
                name: "room_visits");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Visitors");
        }
    }
}
