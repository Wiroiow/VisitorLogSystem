using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLogSystem.Migrations
{
    public partial class AddEmailToVisitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "visitors",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            // Create index for email lookups (performance)
            migrationBuilder.CreateIndex(
                name: "IX_visitors_email",
                table: "visitors",
                column: "email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_visitors_email",
                table: "visitors");

            migrationBuilder.DropColumn(
                name: "email",
                table: "visitors");
        }
    }
}