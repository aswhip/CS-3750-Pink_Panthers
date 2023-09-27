using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pink_Panthers_Project.Migrations
{
    /// <inheritdoc />
    public partial class color : Migration
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
                name: "color",
                table: "Class",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                table: "Class");

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
