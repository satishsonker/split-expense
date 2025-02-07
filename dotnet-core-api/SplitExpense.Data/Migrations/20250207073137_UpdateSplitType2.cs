using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSplitType2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "SplitTypes",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "SplitTypes");
        }
    }
}
