using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class remove_Table_CommentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentTypes_CommentTypeId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "CommentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentTypeId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentTypeId",
                table: "Comments");

            migrationBuilder.AddColumn<long>(
                name: "CommentType",
                table: "Comments",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentType",
                table: "Comments");

            migrationBuilder.AddColumn<long>(
                name: "CommentTypeId",
                table: "Comments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "CommentTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentTypeId",
                table: "Comments",
                column: "CommentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentTypes_CommentTypeId",
                table: "Comments",
                column: "CommentTypeId",
                principalTable: "CommentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
