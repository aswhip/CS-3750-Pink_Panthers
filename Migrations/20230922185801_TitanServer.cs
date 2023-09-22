using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pink_Panthers_Project.Migrations
{
    /// <inheritdoc />
    public partial class TitanServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Days",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "friday",
                table: "Class",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "monday",
                table: "Class",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "thursday",
                table: "Class",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "tuesday",
                table: "Class",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "wednesday",
                table: "Class",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "friday",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "monday",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "thursday",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "tuesday",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "wednesday",
                table: "Class");

            migrationBuilder.AlterColumn<string>(
                name: "Days",
                table: "Class",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
