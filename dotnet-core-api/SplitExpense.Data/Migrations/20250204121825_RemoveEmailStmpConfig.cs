using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitExpense.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailStmpConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailQueues_SmtpSettings_SmtpSettingsId",
                table: "EmailQueues");

            migrationBuilder.DropTable(
                name: "SmtpSettings");

            migrationBuilder.DropIndex(
                name: "IX_EmailQueues_SmtpSettingsId",
                table: "EmailQueues");

            migrationBuilder.DropColumn(
                name: "SmtpSettingsId",
                table: "EmailQueues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SmtpSettingsId",
                table: "EmailQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SmtpSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    EnableSsl = table.Column<bool>(type: "bit", nullable: false),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmtpSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_SmtpSettingsId",
                table: "EmailQueues",
                column: "SmtpSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailQueues_SmtpSettings_SmtpSettingsId",
                table: "EmailQueues",
                column: "SmtpSettingsId",
                principalTable: "SmtpSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
