// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArcaneVaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToCollectionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CollectionItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 1,
                column: "Price",
                value: 89.99m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 2,
                column: "Price",
                value: 24.99m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 3,
                column: "Price",
                value: 45.00m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 4,
                column: "Price",
                value: 34.50m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 5,
                column: "Price",
                value: 18.00m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 6,
                column: "Price",
                value: 55.00m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 7,
                column: "Price",
                value: 299.99m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 8,
                column: "Price",
                value: 22.50m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 9,
                column: "Price",
                value: 79.99m);

            migrationBuilder.UpdateData(
                table: "CollectionItems",
                keyColumn: "ItemId",
                keyValue: 10,
                column: "Price",
                value: 12.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "CollectionItems");
        }
    }
}
