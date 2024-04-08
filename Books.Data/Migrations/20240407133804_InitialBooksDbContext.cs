using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Books.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialBooksDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookSearchHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDataTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SearchQuery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartIndex = table.Column<int>(type: "int", nullable: false),
                    PageSize = table.Column<int>(type: "int", nullable: false),
                    SearchResult = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSearchHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookSearchHistory");
        }
    }
}
