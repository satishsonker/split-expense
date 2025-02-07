using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserGroupMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupMappings_Users_UserId",
                table: "UserGroupMappings");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserGroupMappings",
                newName: "FriendId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupMappings_UserId",
                table: "UserGroupMappings",
                newName: "IX_UserGroupMappings_FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupMappings_CreatedBy",
                table: "UserGroupMappings",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupMappings_Users_CreatedBy",
                table: "UserGroupMappings",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupMappings_Users_FriendId",
                table: "UserGroupMappings",
                column: "FriendId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupMappings_Users_CreatedBy",
                table: "UserGroupMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupMappings_Users_FriendId",
                table: "UserGroupMappings");

            migrationBuilder.DropIndex(
                name: "IX_UserGroupMappings_CreatedBy",
                table: "UserGroupMappings");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "UserGroupMappings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupMappings_FriendId",
                table: "UserGroupMappings",
                newName: "IX_UserGroupMappings_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupMappings_Users_UserId",
                table: "UserGroupMappings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
