using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Students_SId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "SId",
                table: "Answers",
                newName: "EId");

            migrationBuilder.RenameColumn(
                name: "IsCorrect",
                table: "Answers",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_SId",
                table: "Answers",
                newName: "IX_Answers_EId");

            migrationBuilder.AddColumn<string>(
                name: "ExamContent",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentSId",
                table: "Answers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_StudentSId",
                table: "Answers",
                column: "StudentSId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Exams_EId",
                table: "Answers",
                column: "EId",
                principalTable: "Exams",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Students_StudentSId",
                table: "Answers",
                column: "StudentSId",
                principalTable: "Students",
                principalColumn: "SId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Exams_EId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Students_StudentSId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_StudentSId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ExamContent",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StudentSId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Answers",
                newName: "IsCorrect");

            migrationBuilder.RenameColumn(
                name: "EId",
                table: "Answers",
                newName: "SId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_EId",
                table: "Answers",
                newName: "IX_Answers_SId");

            migrationBuilder.AddColumn<int>(
                name: "QId",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QId);
                    table.ForeignKey(
                        name: "FK_Questions_Exams_EId",
                        column: x => x.EId,
                        principalTable: "Exams",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QId",
                table: "Answers",
                column: "QId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_EId",
                table: "Questions",
                column: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QId",
                table: "Answers",
                column: "QId",
                principalTable: "Questions",
                principalColumn: "QId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Students_SId",
                table: "Answers",
                column: "SId",
                principalTable: "Students",
                principalColumn: "SId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
