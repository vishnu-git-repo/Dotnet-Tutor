using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsForPaymentAndLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "AssignedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaymentCompletedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaymentInitiatedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PendingDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PostRemarks",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "PreRemarks",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RazorpayOrderId",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RazorpayPaymentId",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RazorpaySignature",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "WaitlistedDate",
                table: "Borrows");

            migrationBuilder.CreateTable(
                name: "BorrowLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BorrowId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowLogs_Borrows_BorrowId",
                        column: x => x.BorrowId,
                        principalTable: "Borrows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BorrowId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PaymentMode = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RazorpayOrderId = table.Column<string>(type: "text", nullable: true),
                    RazorpayPaymentId = table.Column<string>(type: "text", nullable: true),
                    RazorpaySignature = table.Column<string>(type: "text", nullable: true),
                    PaymentInitiatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentCompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Borrows_BorrowId",
                        column: x => x.BorrowId,
                        principalTable: "Borrows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowLogs_BorrowId",
                table: "BorrowLogs",
                column: "BorrowId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowLogs_UserId",
                table: "BorrowLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BorrowId",
                table: "Payments",
                column: "BorrowId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowLogs");

            migrationBuilder.DropTable(
                name: "Payments");

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
                name: "AssignedDate",
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

            migrationBuilder.AddColumn<int>(
                name: "PaymentMode",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PendingDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostRemarks",
                table: "Borrows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreRemarks",
                table: "Borrows",
                type: "text",
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

            migrationBuilder.AddColumn<string>(
                name: "RazorpaySignature",
                table: "Borrows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WaitlistedDate",
                table: "Borrows",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
