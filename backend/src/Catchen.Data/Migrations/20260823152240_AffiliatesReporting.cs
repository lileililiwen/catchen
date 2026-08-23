using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catchen.Data.Migrations
{
    /// <inheritdoc />
    public partial class AffiliatesReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AffiliateClicks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MerchantSlug = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CampaignId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    VisitorPseudonym = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ClickedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateClicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateMerchant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    BaseUrl = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    AttributionTag = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    RegisteredAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateMerchant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionStatementRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    ExternalRowId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    MerchantSlug = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    AmountMinorUnits = table.Column<long>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RejectReason = table.Column<string>(type: "TEXT", nullable: true),
                    ImportedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionStatementRows", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateClicks_MerchantSlug_ClickedAtUtc",
                table: "AffiliateClicks",
                columns: new[] { "MerchantSlug", "ClickedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateMerchant_Slug",
                table: "AffiliateMerchant",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionStatementRows_Provider_ExternalRowId",
                table: "CommissionStatementRows",
                columns: new[] { "Provider", "ExternalRowId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffiliateClicks");

            migrationBuilder.DropTable(
                name: "AffiliateMerchant");

            migrationBuilder.DropTable(
                name: "CommissionStatementRows");
        }
    }
}
