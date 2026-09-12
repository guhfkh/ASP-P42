using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_P42.Migrations
{
    /// <inheritdoc />
    public partial class добавилновоеполе : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderInPrice",
                table: "ProductGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderInPrice",
                table: "ProductGroups");
        }
    }
}
