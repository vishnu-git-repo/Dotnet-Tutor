using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class AlteredBorrowsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Borrows",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Paid",
                table: "Borrows",
                newName: "PaidAmount");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Borrows",
                newName: "ReturnedCount");

            migrationBuilder.AddColumn<int>(
                name: "EquipmentCount",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "EquipmentPrice",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentCompleted",
                table: "Borrows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LateFee",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentCount",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "EquipmentPrice",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "IsPaymentCompleted",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "LateFee",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Borrows");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Borrows",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "ReturnedCount",
                table: "Borrows",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "Borrows",
                newName: "Paid");
        }
    }
}
