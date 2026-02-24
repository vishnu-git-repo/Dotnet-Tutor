using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _4_InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class Added_the_counts_in_Borrow_relaion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentCounts",
                table: "Borrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentCounts",
                table: "Borrows");
        }
    }
}
