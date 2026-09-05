using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_P42.Migrations
{
    /// <inheritdoc />
    public partial class AddednewmodelsProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHidden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductGroup_ProductGroup_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ProductGroup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHidden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ProductGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Stock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHidden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVersion_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_GroupId",
                table: "Product",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Slug",
                table: "Product",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroup_ParentId",
                table: "ProductGroup",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroup_Slug",
                table: "ProductGroup",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_ProductId",
                table: "ProductVersion",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersion_Slug",
                table: "ProductVersion",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVersion");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductGroup");
        }
    }
}
