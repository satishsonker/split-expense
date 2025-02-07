using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSplitType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SplitTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SplitTypes",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "SplitTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SplitTypes");
        }
    }
}
