using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CrowdSpark.Entitites.Migrations
{
    public partial class attachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Attachments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ProjectId",
                table: "Attachments",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Projects_ProjectId",
                table: "Attachments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Projects_ProjectId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ProjectId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Attachments");
        }
    }
}
