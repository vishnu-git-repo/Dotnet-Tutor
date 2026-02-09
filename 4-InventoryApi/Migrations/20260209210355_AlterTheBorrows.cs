using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class AlterTheBorrows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Borrows");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "Borrows",
                newName: "WaitlistedDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Borrows",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAmount",
                table: "Borrows",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "LateFee",
                table: "Borrows",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "EquipmentPrice",
                table: "Borrows",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "DueAmount",
                table: "Borrows",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AckDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AckRemarks",
                table: "Borrows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssingnedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Borrows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PendingDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "AckDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "AckRemarks",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "AssingnedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PendingDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Borrows");

            migrationBuilder.RenameColumn(
                name: "WaitlistedDate",
                table: "Borrows",
                newName: "PaymentDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAmount",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "LateFee",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "EquipmentPrice",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "DueAmount",
                table: "Borrows",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
