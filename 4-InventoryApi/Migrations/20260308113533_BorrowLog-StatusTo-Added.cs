using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class BorrowLogStatusToAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "BorrowLogs",
                newName: "StatusTo");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "BorrowLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "StatusFrom",
                table: "BorrowLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs");

            migrationBuilder.DropColumn(
                name: "StatusFrom",
                table: "BorrowLogs");

            migrationBuilder.RenameColumn(
                name: "StatusTo",
                table: "BorrowLogs",
                newName: "Status");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "BorrowLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowLogs_Users_UserId",
                table: "BorrowLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
