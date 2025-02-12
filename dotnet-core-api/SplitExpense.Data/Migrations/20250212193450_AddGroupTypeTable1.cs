using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupTypeTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupType_GroupTypeId",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupType",
                table: "GroupType");

            migrationBuilder.RenameTable(
                name: "GroupType",
                newName: "GroupTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupTypes",
                table: "GroupTypes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GroupDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    EnableGroupDate = table.Column<bool>(type: "bit", nullable: false),
                    EnableSettleUpReminders = table.Column<bool>(type: "bit", nullable: false),
                    EnableBalanceAlert = table.Column<bool>(type: "bit", nullable: false),
                    MaxGroupBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_GroupDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupDetails_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupDetails_GroupId",
                table: "GroupDetails",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupTypes_GroupTypeId",
                table: "Groups",
                column: "GroupTypeId",
                principalTable: "GroupTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupTypes_GroupTypeId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "GroupDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupTypes",
                table: "GroupTypes");

            migrationBuilder.RenameTable(
                name: "GroupTypes",
                newName: "GroupType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupType",
                table: "GroupType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupType_GroupTypeId",
                table: "Groups",
                column: "GroupTypeId",
                principalTable: "GroupType",
                principalColumn: "Id");
        }
    }
}
