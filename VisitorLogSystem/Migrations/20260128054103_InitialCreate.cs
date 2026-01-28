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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visitors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    contact_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    time_in = table.Column<DateTime>(type: "datetime2", nullable: false),
                    time_out = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitors", x => x.id);
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

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visitors_time_in",
                table: "visitors",
                column: "time_in");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_visits");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "visitors");
        }
    }
}
