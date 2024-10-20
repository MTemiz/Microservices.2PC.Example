using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("08a86647-5766-425d-a48e-e5e7ce9518f5"), "Payment.Api" },
                    { new Guid("32284026-1aee-4593-88f1-7a2b2c4cb7cb"), "Stock.Api" },
                    { new Guid("abc8223b-c7fd-4ede-beae-e0fc4b31a12b"), "Order.Api" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("08a86647-5766-425d-a48e-e5e7ce9518f5"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("32284026-1aee-4593-88f1-7a2b2c4cb7cb"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("abc8223b-c7fd-4ede-beae-e0fc4b31a12b"));
        }
    }
}
