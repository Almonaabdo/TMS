using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Cities_CitiesCityId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Cities_CitiesCityId1",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CitiesCityId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CitiesCityId1",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CitiesCityId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CitiesCityId1",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DestinationCityId",
                table: "Order",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_SourceCityId",
                table: "Order",
                column: "SourceCityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Cities_DestinationCityId",
                table: "Order",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Cities_SourceCityId",
                table: "Order",
                column: "SourceCityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Cities_DestinationCityId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Cities_SourceCityId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_DestinationCityId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_SourceCityId",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "CitiesCityId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CitiesCityId1",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_CitiesCityId",
                table: "Order",
                column: "CitiesCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CitiesCityId1",
                table: "Order",
                column: "CitiesCityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Cities_CitiesCityId",
                table: "Order",
                column: "CitiesCityId",
                principalTable: "Cities",
                principalColumn: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Cities_CitiesCityId1",
                table: "Order",
                column: "CitiesCityId1",
                principalTable: "Cities",
                principalColumn: "CityId");
        }
    }
}
