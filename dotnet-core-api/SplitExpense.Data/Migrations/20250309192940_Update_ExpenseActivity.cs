using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ExpenseActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ExpenseActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseActivities_UserId",
                table: "ExpenseActivities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseActivities_Users_UserId",
                table: "ExpenseActivities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseActivities_Users_UserId",
                table: "ExpenseActivities");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseActivities_UserId",
                table: "ExpenseActivities");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ExpenseActivities");
        }
    }
}
