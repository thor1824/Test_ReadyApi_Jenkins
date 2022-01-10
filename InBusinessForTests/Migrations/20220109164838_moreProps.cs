using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InBusinessForTests.Migrations
{
    public partial class moreProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountBoughtForDiscount",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountInPercentage",
                table: "Products",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfPurchase",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DiscountAmountAtTimeOfPurchase",
                table: "OrderLines",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountAtTimeOfPurchase",
                table: "OrderLines",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceAtTimeOfPurchase",
                table: "OrderLines",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountBoughtForDiscount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountInPercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TimeOfPurchase",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmountAtTimeOfPurchase",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "DiscountAtTimeOfPurchase",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "PriceAtTimeOfPurchase",
                table: "OrderLines");
        }
    }
}
