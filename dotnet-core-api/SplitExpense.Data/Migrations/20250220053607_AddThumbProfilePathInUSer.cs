using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbProfilePathInUSer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbProfilePicture",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbProfilePicture",
                table: "Users");
        }
    }
}
