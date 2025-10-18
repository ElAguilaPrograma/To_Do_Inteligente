using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_ToDo.Migrations
{
    /// <inheritdoc />
    public partial class AddHangfireJobIdToTasksSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HangfireJobId",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HangfireJobId",
                table: "Tasks");
        }
    }
}
