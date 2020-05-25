using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnackExchange.Web.Migrations
{
    public partial class OfferIdProduck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "OfferId",
                table: "Products",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "OfferId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
