using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catchen.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogEditorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishedRecipe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Cuisine = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviewText = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ContentJson = table.Column<string>(type: "TEXT", nullable: false),
                    IngredientIndex = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReviewerUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PublishedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedRecipe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ModerationReason = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeComment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeDraft",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Cuisine = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Difficulty = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    IsFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContentJson = table.Column<string>(type: "TEXT", nullable: false),
                    ProvenanceJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReviewerUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SecondaryReviewAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    SecondaryReviewerUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ValidationReportJson = table.Column<string>(type: "TEXT", nullable: true),
                    PublishedRecipeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PublishedVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeDraft", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeFavorite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeFavorite", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedRecipe_IsLive_Cuisine_Difficulty",
                table: "PublishedRecipe",
                columns: new[] { "IsLive", "Cuisine", "Difficulty" });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedRecipe_RecipeId_Version",
                table: "PublishedRecipe",
                columns: new[] { "RecipeId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeComment_RecipeId_Status",
                table: "RecipeComment",
                columns: new[] { "RecipeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeComment_UserId",
                table: "RecipeComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDraft_Status",
                table: "RecipeDraft",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFavorite_UserId_RecipeId",
                table: "RecipeFavorite",
                columns: new[] { "UserId", "RecipeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedRecipe");

            migrationBuilder.DropTable(
                name: "RecipeComment");

            migrationBuilder.DropTable(
                name: "RecipeDraft");

            migrationBuilder.DropTable(
                name: "RecipeFavorite");
        }
    }
}
