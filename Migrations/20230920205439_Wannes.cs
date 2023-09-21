using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pink_Panthers_Project.Migrations
{
    /// <inheritdoc />
    public partial class Wannes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Account",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Account",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Account");
        }
    }
}
