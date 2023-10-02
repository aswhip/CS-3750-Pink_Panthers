using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pink_Panthers_Project.Migrations
{
    /// <inheritdoc />
    public partial class creditHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "hours",
                table: "Class",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "hours",
                table: "Class");
        }
    }
}
