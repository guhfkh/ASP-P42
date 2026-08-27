using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_P42.Migrations
{
    /// <inheritdoc />
    public partial class updateDK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserAccesses",
                keyColumn: "Id",
                keyValue: new Guid("96dcbbba-9aee-44a2-8835-72dfe4e1a710"),
                column: "Dk",
                value: "FCB57CECE720632FDBB68958CF953E46");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserAccesses",
                keyColumn: "Id",
                keyValue: new Guid("96dcbbba-9aee-44a2-8835-72dfe4e1a710"),
                column: "Dk",
                value: "");
        }
    }
}
