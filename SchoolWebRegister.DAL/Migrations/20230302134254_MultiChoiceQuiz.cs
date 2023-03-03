using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebRegister.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MultiChoiceQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLection_Course_CourseId",
                table: "CourseLection");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_CourseLection_CourseLectionId",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_QuizQuestion_QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_QuizQuestion_QuizQuestionQuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestion_Quiz_QuizId",
                table: "QuizQuestion");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswer_QuizQuestionQuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CourseStudying_StudentId_CourseId",
                table: "CourseStudying");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStudying",
                table: "CourseStudying");

            migrationBuilder.DropColumn(
                name: "QuizQuestionQuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseStudying");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "CourseStudying",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "QuizQuestion",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MultiChoice",
                table: "QuizQuestion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionId",
                table: "QuizAnswer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseLectionId",
                table: "Quiz",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "CourseLection",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStudying",
                table: "CourseStudying",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestion_QuestionId",
                table: "QuizQuestion",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswer_AnswerId",
                table: "QuizAnswer",
                column: "AnswerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CourseStudying_Id",
                table: "AspNetUsers",
                column: "Id",
                principalTable: "CourseStudying",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseStudying_Id",
                table: "Course",
                column: "Id",
                principalTable: "CourseStudying",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLection_Course_CourseId",
                table: "CourseLection",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_CourseLection_CourseLectionId",
                table: "Quiz",
                column: "CourseLectionId",
                principalTable: "CourseLection",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_QuizQuestion_QuestionId",
                table: "QuizAnswer",
                column: "QuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestion_Quiz_QuizId",
                table: "QuizQuestion",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CourseStudying_Id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseStudying_Id",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseLection_Course_CourseId",
                table: "CourseLection");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_CourseLection_CourseLectionId",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_QuizQuestion_QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestion_Quiz_QuizId",
                table: "QuizQuestion");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestion_QuestionId",
                table: "QuizQuestion");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswer_AnswerId",
                table: "QuizAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStudying",
                table: "CourseStudying");

            migrationBuilder.DropColumn(
                name: "MultiChoice",
                table: "QuizQuestion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CourseStudying",
                newName: "StudentId");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "QuizQuestion",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionId",
                table: "QuizAnswer",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "QuizQuestionQuestionId",
                table: "QuizAnswer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CourseLectionId",
                table: "Quiz",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "CourseStudying",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "CourseLection",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CourseStudying_StudentId_CourseId",
                table: "CourseStudying",
                columns: new[] { "StudentId", "CourseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStudying",
                table: "CourseStudying",
                columns: new[] { "CourseId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswer_QuizQuestionQuestionId",
                table: "QuizAnswer",
                column: "QuizQuestionQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLection_Course_CourseId",
                table: "CourseLection",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_CourseLection_CourseLectionId",
                table: "Quiz",
                column: "CourseLectionId",
                principalTable: "CourseLection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_QuizQuestion_QuestionId",
                table: "QuizAnswer",
                column: "QuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_QuizQuestion_QuizQuestionQuestionId",
                table: "QuizAnswer",
                column: "QuizQuestionQuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestion_Quiz_QuizId",
                table: "QuizQuestion",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id");
        }
    }
}
