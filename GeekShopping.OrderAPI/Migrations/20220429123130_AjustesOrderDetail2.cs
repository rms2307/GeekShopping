using Microsoft.EntityFrameworkCore.Migrations;

namespace GeekShopping.OrderAPI.Migrations
{
    public partial class AjustesOrderDetail2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_detail_order_header_order_header_id",
                table: "order_detail");

            migrationBuilder.DropIndex(
                name: "IX_order_detail_order_header_id",
                table: "order_detail");

            migrationBuilder.DropColumn(
                name: "order_header_id",
                table: "order_detail");

            migrationBuilder.CreateIndex(
                name: "IX_order_detail_OrderHeaderId",
                table: "order_detail",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_order_detail_order_header_OrderHeaderId",
                table: "order_detail",
                column: "OrderHeaderId",
                principalTable: "order_header",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_detail_order_header_OrderHeaderId",
                table: "order_detail");

            migrationBuilder.DropIndex(
                name: "IX_order_detail_OrderHeaderId",
                table: "order_detail");

            migrationBuilder.AddColumn<long>(
                name: "order_header_id",
                table: "order_detail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_order_detail_order_header_id",
                table: "order_detail",
                column: "order_header_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_detail_order_header_order_header_id",
                table: "order_detail",
                column: "order_header_id",
                principalTable: "order_header",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
