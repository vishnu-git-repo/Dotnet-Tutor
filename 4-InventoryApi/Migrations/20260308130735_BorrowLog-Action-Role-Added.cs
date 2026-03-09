using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class BorrowLogActionRoleAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "BorrowLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "BorrowLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "BorrowLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "BorrowLogs");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "BorrowLogs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "BorrowLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
