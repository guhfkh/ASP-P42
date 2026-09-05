using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_P42.Migrations
{
    /// <inheritdoc />
    public partial class Remene : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductGroup_GroupId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroup_ProductGroup_ParentId",
                table: "ProductGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersion_Product_ProductId",
                table: "ProductVersion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVersion",
                table: "ProductVersion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductGroup",
                table: "ProductGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "ProductVersion",
                newName: "ProductVersions");

            migrationBuilder.RenameTable(
                name: "ProductGroup",
                newName: "ProductGroups");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVersion_Slug",
                table: "ProductVersions",
                newName: "IX_ProductVersions_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVersion_ProductId",
                table: "ProductVersions",
                newName: "IX_ProductVersions_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroup_Slug",
                table: "ProductGroups",
                newName: "IX_ProductGroups_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroup_ParentId",
                table: "ProductGroups",
                newName: "IX_ProductGroups_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Slug",
                table: "Products",
                newName: "IX_Products_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Product_GroupId",
                table: "Products",
                newName: "IX_Products_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVersions",
                table: "ProductVersions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductGroups",
                table: "ProductGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroups_ProductGroups_ParentId",
                table: "ProductGroups",
                column: "ParentId",
                principalTable: "ProductGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductGroups_GroupId",
                table: "Products",
                column: "GroupId",
                principalTable: "ProductGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersions_Products_ProductId",
                table: "ProductVersions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroups_ProductGroups_ParentId",
                table: "ProductGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductGroups_GroupId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVersions_Products_ProductId",
                table: "ProductVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVersions",
                table: "ProductVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductGroups",
                table: "ProductGroups");

            migrationBuilder.RenameTable(
                name: "ProductVersions",
                newName: "ProductVersion");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductGroups",
                newName: "ProductGroup");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVersions_Slug",
                table: "ProductVersion",
                newName: "IX_ProductVersion_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVersions_ProductId",
                table: "ProductVersion",
                newName: "IX_ProductVersion_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Slug",
                table: "Product",
                newName: "IX_Product_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Products_GroupId",
                table: "Product",
                newName: "IX_Product_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroups_Slug",
                table: "ProductGroup",
                newName: "IX_ProductGroup_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroups_ParentId",
                table: "ProductGroup",
                newName: "IX_ProductGroup_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVersion",
                table: "ProductVersion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductGroup",
                table: "ProductGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductGroup_GroupId",
                table: "Product",
                column: "GroupId",
                principalTable: "ProductGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroup_ProductGroup_ParentId",
                table: "ProductGroup",
                column: "ParentId",
                principalTable: "ProductGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVersion_Product_ProductId",
                table: "ProductVersion",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
