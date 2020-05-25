using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnackExchange.Web.Migrations
{
    public partial class Nalible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "OfferId",
                table: "Products",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "OfferId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Offer_OfferId",
                table: "Products",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
