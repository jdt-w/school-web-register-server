using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebRegister.DAL.Migrations
{
    /// <inheritdoc />
    public partial class QuizPublishingTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "PassThreshold",
                table: "Quiz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartsAt",
                table: "Quiz",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndsAt",
                table: "Quiz",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddCheckConstraint(
                name: "PassThreshold",
                table: "Quiz",
                sql: "PassThreshold > 0 AND PassThreshold < 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "PassThreshold",
                table: "Quiz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartsAt",
                table: "Quiz",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndsAt",
                table: "Quiz",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "PassThreshold",
                table: "Quiz",
                sql: "CHECK (PassThreshold > 0 AND PassThreshold < 1)");
        }
    }
}
