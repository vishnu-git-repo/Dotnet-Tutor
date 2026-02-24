using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class Addedborrowitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BorrowItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentItemId = table.Column<int>(type: "integer", nullable: false),
                    BorrowId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowItems_Borrows_BorrowId",
                        column: x => x.BorrowId,
                        principalTable: "Borrows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowItems_EquipmentItems_EquipmentItemId",
                        column: x => x.EquipmentItemId,
                        principalTable: "EquipmentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowItems_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowItems_BorrowId",
                table: "BorrowItems",
                column: "BorrowId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowItems_EquipmentId",
                table: "BorrowItems",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowItems_EquipmentItemId",
                table: "BorrowItems",
                column: "EquipmentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowItems_UserId",
                table: "BorrowItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowItems");
        }
    }
}
