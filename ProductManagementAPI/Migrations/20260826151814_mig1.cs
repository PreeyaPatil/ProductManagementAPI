using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Name", "Price", "Sku", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "15-inch business laptop", true, "Laptop", 60000.00m, "LAPTOP-001", 20, null },
                    { 2, "Electronics", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "5G Android smartphone", true, "Smartphone", 30000.00m, "PHONE-001", 40, null },
                    { 3, "Accessories", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wireless optical mouse", true, "Wireless Mouse", 1500.00m, "MOUSE-001", 100, null },
                    { 4, "Accessories", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mechanical keyboard with backlight", true, "Mechanical Keyboard", 4500.00m, "KEYBOARD-001", 50, null },
                    { 5, "Electronics", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24-inch Full HD LED monitor", true, "LED Monitor", 18000.00m, "MONITOR-001", 30, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
