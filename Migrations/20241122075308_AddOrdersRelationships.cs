using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_BookPvt.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "BookID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Processing");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BookID",
                table: "Orders",
                column: "BookID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Books_BookID",
                table: "Orders",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Books_BookID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BookID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BookID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
