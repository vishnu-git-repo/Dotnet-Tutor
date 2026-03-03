using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class addrazorpayidInBorrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Borrows",
                newName: "RazorpaySignature");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentCompletedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentInitiatedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazorpayOrderId",
                table: "Borrows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazorpayPaymentId",
                table: "Borrows",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCompletedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaymentInitiatedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RazorpayOrderId",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RazorpayPaymentId",
                table: "Borrows");

            migrationBuilder.RenameColumn(
                name: "RazorpaySignature",
                table: "Borrows",
                newName: "PaymentId");
        }
    }
}
