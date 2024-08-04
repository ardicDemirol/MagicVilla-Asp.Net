using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVillaVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedVillaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "CreatedDate", "Details", "ImageUrl", "Name", "Rate", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 4, 15, 25, 29, 532, DateTimeKind.Local).AddTicks(6164), "Villa 1 Description", "https://via.placeholder.com/150", "Villa 1", 1000.0, new DateTime(2024, 8, 4, 15, 25, 29, 532, DateTimeKind.Local).AddTicks(6176) },
                    { 2, new DateTime(2024, 8, 4, 15, 25, 29, 532, DateTimeKind.Local).AddTicks(6180), "Villa 2 Description", "https://via.placeholder.com/150", "Villa 2", 2000.0, new DateTime(2024, 8, 4, 15, 25, 29, 532, DateTimeKind.Local).AddTicks(6180) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
