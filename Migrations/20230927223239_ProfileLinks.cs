using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pink_Panthers_Project.Migrations
{
    /// <inheritdoc />
    public partial class ProfileLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_Account_accountID",
                table: "Class");

            migrationBuilder.DropIndex(
                name: "IX_Class_accountID",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "Class");

            migrationBuilder.AddColumn<string>(
                name: "ProfileLink1",
                table: "Account",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileLink2",
                table: "Account",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileLink3",
                table: "Account",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileLink1",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ProfileLink2",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ProfileLink3",
                table: "Account");

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "Class",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Class_accountID",
                table: "Class",
                column: "accountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Account_accountID",
                table: "Class",
                column: "accountID",
                principalTable: "Account",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
