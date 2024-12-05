using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addStreetAddressInCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "Companies");
        }
    }
}
