using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Danek.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "CreatedAt", "Description", "Intro", "Quantity", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Robert C. Martin", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic book about writing clean, maintainable and readable code.", "A Handbook of Agile Software Craftsmanship.", 5, "Clean Code" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Jeffrey Richter", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced book for experienced .NET developers.", "Deep dive into .NET CLR and C# internals.", 3, "CLR via C#" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Andrew Lock", new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Practical guide to building web apps with ASP.NET Core.", 0, "ASP.NET Core in Action" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}
