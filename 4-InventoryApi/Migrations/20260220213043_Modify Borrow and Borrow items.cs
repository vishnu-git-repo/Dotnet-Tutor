using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class ModifyBorrowandBorrowitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowItems_Users_UserId",
                table: "BorrowItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Borrows_Equipments_EquipmentId",
                table: "Borrows");

            migrationBuilder.DropIndex(
                name: "IX_Borrows_EquipmentId",
                table: "Borrows");

            migrationBuilder.DropIndex(
                name: "IX_BorrowItems_UserId",
                table: "BorrowItems");

            migrationBuilder.DropColumn(
                name: "BorrowedDays",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "EquipmentCount",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "EquipmentPrice",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ReturnedCount",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BorrowItems");

            migrationBuilder.RenameColumn(
                name: "AssingnedDate",
                table: "Borrows",
                newName: "AssignedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualReturnDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedReturnDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "EquipmentPrice",
                table: "BorrowItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "BorrowItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedAt",
                table: "BorrowItems",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualReturnDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ExpectedReturnDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "EquipmentPrice",
                table: "BorrowItems");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "BorrowItems");

            migrationBuilder.DropColumn(
                name: "ReturnedAt",
                table: "BorrowItems");

            migrationBuilder.RenameColumn(
                name: "AssignedDate",
                table: "Borrows",
                newName: "AssingnedDate");

            migrationBuilder.AddColumn<int>(
                name: "BorrowedDays",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentCount",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "EquipmentPrice",
                table: "Borrows",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ReturnedCount",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BorrowItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Borrows_EquipmentId",
                table: "Borrows",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowItems_UserId",
                table: "BorrowItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowItems_Users_UserId",
                table: "BorrowItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Borrows_Equipments_EquipmentId",
                table: "Borrows",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
