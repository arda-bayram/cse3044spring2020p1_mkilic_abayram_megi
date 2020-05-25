using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnackExchange.Web.Migrations
{
    public partial class ImproveExchangeOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OfferId",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    ExchangeId = table.Column<Guid>(nullable: true),
                    OffererId = table.Column<string>(nullable: true),
                    OfferNotes = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offer_Exchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_AspNetUsers_OffererId",
                        column: x => x.OffererId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_OfferId",
                table: "Products",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_ExchangeId",
                table: "Offer",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_OffererId",
                table: "Offer",
                column: "OffererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Products_OfferId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "Products");
        }
    }
}
