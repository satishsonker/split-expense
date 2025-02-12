using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserGroupMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "UserGroupMappings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SplitTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "SplitTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Groups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupTypeId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Friends",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Friends",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExpenseShares",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ExpenseShares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Expenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExpenseReceipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ExpenseReceipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExpenseNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ExpenseNotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ErrorLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ErrorLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EmailTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "EmailTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EmailQueues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "EmailQueues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupTypeId",
                table: "Groups",
                column: "GroupTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupType_GroupTypeId",
                table: "Groups",
                column: "GroupTypeId",
                principalTable: "GroupType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupType_GroupTypeId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "GroupType");

            migrationBuilder.DropIndex(
                name: "IX_Groups_GroupTypeId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserGroupMappings");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "UserGroupMappings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SplitTypes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SplitTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupTypeId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExpenseShares");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExpenseShares");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExpenseReceipts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExpenseReceipts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExpenseNotes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExpenseNotes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EmailQueues");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "EmailQueues");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Contacts");
        }
    }
}
