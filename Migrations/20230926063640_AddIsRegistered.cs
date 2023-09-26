using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pink_Panthers_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRegistered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "Class",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_registeredClasses_accountID",
                table: "registeredClasses",
                column: "accountID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_registeredClasses_Account_accountID",
                table: "registeredClasses",
                column: "accountID",
                principalTable: "Account",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_Account_accountID",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_registeredClasses_Account_accountID",
                table: "registeredClasses");

            migrationBuilder.DropIndex(
                name: "IX_registeredClasses_accountID",
                table: "registeredClasses");

            migrationBuilder.DropIndex(
                name: "IX_Class_accountID",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "Class");
        }
    }
}
