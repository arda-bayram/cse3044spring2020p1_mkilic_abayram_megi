using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnackExchange.Web.Migrations
{
    public partial class OfferExchangeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeId",
                table: "Offer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer",
                column: "ExchangeId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeId",
                table: "Offer",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer",
                column: "ExchangeId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
