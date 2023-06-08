using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QuizzesIdUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QuizEntityQuizItemEntity",
                columns: new[] { "ItemsId", "QuizzesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QuizEntityQuizItemEntity",
                keyColumns: new[] { "ItemsId", "QuizzesId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "QuizEntityQuizItemEntity",
                keyColumns: new[] { "ItemsId", "QuizzesId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "QuizEntityQuizItemEntity",
                keyColumns: new[] { "ItemsId", "QuizzesId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "QuizEntityQuizItemEntity",
                keyColumns: new[] { "ItemsId", "QuizzesId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "QuizEntityQuizItemEntity",
                keyColumns: new[] { "ItemsId", "QuizzesId" },
                keyValues: new object[] { 4, 2 });
        }
    }
}
