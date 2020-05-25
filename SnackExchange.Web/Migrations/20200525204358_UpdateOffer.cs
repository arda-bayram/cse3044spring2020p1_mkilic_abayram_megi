using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnackExchange.Web.Migrations
{
    public partial class UpdateOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExchangeId",
                table: "Offer",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer",
                column: "ExchangeId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Exchanges_ExchangeId",
                table: "Offer",
                column: "ExchangeId",
                principalTable: "Exchanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
