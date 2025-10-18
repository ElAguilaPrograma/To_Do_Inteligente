using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_ToDo.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderDateToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderDate",
                table: "Tasks");
        }
    }
}
