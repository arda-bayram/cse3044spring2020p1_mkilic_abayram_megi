using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnackExchange.Web.Migrations
{
    public partial class AddNewFieldsToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Exchanges",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "Exchanges",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ModeratorId",
                table: "Exchanges",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Exchanges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "Addresses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExchangeUserModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UserExchangeRole = table.Column<int>(nullable: false),
                    ExchangeId = table.Column<Guid>(nullable: true),
                    AppUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeUserModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeUserModel_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeUserModel_Exchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_ModeratorId",
                table: "Exchanges",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_ReceiverId",
                table: "Exchanges",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_SenderId",
                table: "Exchanges",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeUserModel_AppUserId",
                table: "ExchangeUserModel",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeUserModel_ExchangeId",
                table: "ExchangeUserModel",
                column: "ExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Countries_CountryId",
                table: "AspNetUsers",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_AspNetUsers_ModeratorId",
                table: "Exchanges",
                column: "ModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_AspNetUsers_ReceiverId",
                table: "Exchanges",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_AspNetUsers_SenderId",
                table: "Exchanges",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Countries_CountryId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_AspNetUsers_ModeratorId",
                table: "Exchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_AspNetUsers_ReceiverId",
                table: "Exchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_AspNetUsers_SenderId",
                table: "Exchanges");

            migrationBuilder.DropTable(
                name: "ExchangeUserModel");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_ModeratorId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_ReceiverId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_SenderId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Exchanges");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Addresses");

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Exchanges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverId",
                table: "Exchanges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModeratorId",
                table: "Exchanges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
