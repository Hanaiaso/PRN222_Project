using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PRN_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateData3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ranks_Exams_EId",
                table: "Ranks");

            migrationBuilder.DropForeignKey(
                name: "FK_Ranks_Students_SId",
                table: "Ranks");

            migrationBuilder.DropIndex(
                name: "IX_Ranks_EId",
                table: "Ranks");

            migrationBuilder.DropIndex(
                name: "IX_Ranks_SId",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "EId",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "RankOrder",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "SId",
                table: "Ranks");

            migrationBuilder.AddColumn<float>(
                name: "MaxScore",
                table: "Ranks",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MinScore",
                table: "Ranks",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamRanks",
                columns: table => new
                {
                    ErId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SId = table.Column<int>(type: "int", nullable: false),
                    EId = table.Column<int>(type: "int", nullable: false),
                    RaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamRanks", x => x.ErId);
                    table.ForeignKey(
                        name: "FK_ExamRanks_Exams_EId",
                        column: x => x.EId,
                        principalTable: "Exams",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamRanks_Ranks_RaId",
                        column: x => x.RaId,
                        principalTable: "Ranks",
                        principalColumn: "RaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamRanks_Students_SId",
                        column: x => x.SId,
                        principalTable: "Students",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AId", "Email", "Password", "Role", "Status" },
                values: new object[,]
                {
                    { 1, "admin@example.com", "Admin@123", 0, true },
                    { 2, "teacher@example.com", "Teacher@123", 1, true },
                    { 3, "student@example.com", "Student@123", 2, true },
                    { 4, "student2@example.com", "Student@123", 2, true }
                });

            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "EId", "CreatedAt", "EName", "EndTime", "ExamContent", "StartTime", "Status", "SuId", "SubjectSuId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 22, 51, 36, 930, DateTimeKind.Local).AddTicks(3489), "Math Midterm Exam", new DateTime(2025, 10, 24, 0, 51, 36, 931, DateTimeKind.Local).AddTicks(5305), "[\r\n                        {\r\n                            \"Question\": \"What is 2 + 2?\",\r\n                            \"Options\": [\"3\", \"4\", \"5\", \"6\"],\r\n                            \"CorrectAnswer\": \"4\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"What is 10 / 2?\",\r\n                            \"Options\": [\"2\", \"5\", \"10\", \"20\"],\r\n                            \"CorrectAnswer\": \"5\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 23, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(4978), true, 1, null },
                    { 2, new DateTime(2025, 10, 22, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(5799), "English Grammar Test", new DateTime(2025, 10, 25, 23, 51, 36, 931, DateTimeKind.Local).AddTicks(5802), "[\r\n                        {\r\n                            \"Question\": \"Choose the correct form: He ___ to school every day.\",\r\n                            \"Options\": [\"go\", \"goes\", \"going\", \"gone\"],\r\n                            \"CorrectAnswer\": \"goes\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"Which word is a noun?\",\r\n                            \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"],\r\n                            \"CorrectAnswer\": \"happiness\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 25, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(5800), true, 2, null }
                });

            migrationBuilder.InsertData(
                table: "Ranks",
                columns: new[] { "RaId", "MaxScore", "MinScore", "RankName" },
                values: new object[,]
                {
                    { 1, 10f, 8.5f, "A" },
                    { 2, 8.4f, 6.5f, "B" },
                    { 3, 6.4f, 4.5f, "C" },
                    { 4, 4.4f, 0f, "D" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "SuId", "SuName" },
                values: new object[,]
                {
                    { 1, "Mathematics" },
                    { 2, "English" }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdId", "AId", "AdName" },
                values: new object[] { 1, 1, "Super Admin" });

            migrationBuilder.InsertData(
                table: "Answers",
                columns: new[] { "AwId", "AnswerContent", "CreatedAt", "EId", "Status", "StudentSId" },
                values: new object[,]
                {
                    { 1, "[\r\n                        {\r\n                            \"Question\": \"What is 2 + 2?\",\r\n                            \"Options\": [\"3\", \"4\", \"5\", \"6\"],\r\n                            \"CorrectAnswer\": \"4\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"What is 10 / 2?\",\r\n                            \"Options\": [\"2\", \"5\", \"10\", \"20\"],\r\n                            \"CorrectAnswer\": \"5\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 22, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(7780), 1, true, null },
                    { 2, "[\r\n                        {\r\n                            \"Question\": \"Choose the correct form: He ___ to school every day.\",\r\n                            \"Options\": [\"go\", \"goes\", \"going\", \"gone\"],\r\n                            \"CorrectAnswer\": \"goes\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"Which word is a noun?\",\r\n                            \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"],\r\n                            \"CorrectAnswer\": \"happiness\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 22, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(8281), 2, true, null }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NtId", "Content", "SenderId", "SentTime", "Title" },
                values: new object[,]
                {
                    { 1, "We’re excited to have you here! Start exploring the exam system and check your profile for details.", 1, new DateTime(2025, 10, 17, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(1727), "Welcome to the LMS System" },
                    { 2, "Don't forget! The Math midterm exam will take place this Friday at 8:00 AM.", 2, new DateTime(2025, 10, 20, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(1998), "Upcoming Math Exam Reminder" },
                    { 3, "The LMS system will be under maintenance this weekend. Please save your progress beforehand.", 1, new DateTime(2025, 10, 21, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(2000), "System Maintenance" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "SId", "AId", "Dob", "Gender", "SName" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2008, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Female", "Tran Thi C" },
                    { 2, 4, new DateTime(2008, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Le Van D" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "TId", "AId", "Qualification", "TName" },
                values: new object[] { 1, 2, "Master of Mathematics", "Nguyen Van B" });

            migrationBuilder.InsertData(
                table: "ExamRanks",
                columns: new[] { "ErId", "EId", "RaId", "SId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 2, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "NotificationReceivers",
                columns: new[] { "NrId", "IsRead", "NtId", "ReceiverId" },
                values: new object[,]
                {
                    { 1, true, 1, 3 },
                    { 2, false, 1, 4 },
                    { 3, false, 2, 3 },
                    { 4, true, 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "Submits",
                columns: new[] { "SbId", "Comment", "Content", "EId", "SId", "Score", "SubmitTime" },
                values: new object[,]
                {
                    { 1, "Good performance, minor mistakes.", "[\r\n                        {\r\n                            \"QuestionIndex\": 1,\r\n                            \"ChosenAnswer\": \"4\"\r\n                        },\r\n                        {\r\n                            \"QuestionIndex\": 2,\r\n                            \"ChosenAnswer\": \"5\"\r\n                        }\r\n                    ]", 1, 1, 9.0, new DateTime(2025, 10, 21, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(9782) },
                    { 2, "Needs to improve grammar.", "[\r\n                        {\r\n                            \"QuestionIndex\": 1,\r\n                            \"ChosenAnswer\": \"goes\"\r\n                        },\r\n                        {\r\n                            \"QuestionIndex\": 2,\r\n                            \"ChosenAnswer\": \"run\"\r\n                        }\r\n                    ]", 2, 2, 7.5, new DateTime(2025, 10, 20, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(211) }
                });

            migrationBuilder.InsertData(
                table: "TeacherSubjects",
                columns: new[] { "TsId", "SuId", "TId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_EId",
                table: "ExamRanks",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_RaId",
                table: "ExamRanks",
                column: "RaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_SId",
                table: "ExamRanks",
                column: "SId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamRanks");

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "AwId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "AwId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ranks",
                keyColumn: "RaId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ranks",
                keyColumn: "RaId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ranks",
                keyColumn: "RaId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ranks",
                keyColumn: "RaId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Submits",
                keyColumn: "SbId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Submits",
                keyColumn: "SbId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TeacherSubjects",
                keyColumn: "TsId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TeacherSubjects",
                keyColumn: "TsId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Exams",
                keyColumn: "EId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exams",
                keyColumn: "EId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "SId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "SId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SuId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SuId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "TId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AId",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "MinScore",
                table: "Ranks");

            migrationBuilder.AddColumn<int>(
                name: "EId",
                table: "Ranks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RankOrder",
                table: "Ranks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SId",
                table: "Ranks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_EId",
                table: "Ranks",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_SId",
                table: "Ranks",
                column: "SId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ranks_Exams_EId",
                table: "Ranks",
                column: "EId",
                principalTable: "Exams",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ranks_Students_SId",
                table: "Ranks",
                column: "SId",
                principalTable: "Students",
                principalColumn: "SId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
