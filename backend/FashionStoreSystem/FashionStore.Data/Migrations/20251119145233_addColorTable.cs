using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class addColorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_ProductId_SizeId",
                table: "ProductSizes");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_ColorId",
                table: "ProductSizes",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_ProductId_SizeId_ColorId",
                table: "ProductSizes",
                columns: new[] { "ProductId", "SizeId", "ColorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ColorId",
                table: "OrderDetails",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Colors_ColorId",
                table: "OrderDetails",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_Colors_ColorId",
                table: "ProductSizes",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Colors_ColorId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_Colors_ColorId",
                table: "ProductSizes");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_ColorId",
                table: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_ProductId_SizeId_ColorId",
                table: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ColorId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_ProductId_SizeId",
                table: "ProductSizes",
                columns: new[] { "ProductId", "SizeId" },
                unique: true);
        }
    }
}
