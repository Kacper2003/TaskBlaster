using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBlaster.TaskManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Tasks");
        }
    }
}
