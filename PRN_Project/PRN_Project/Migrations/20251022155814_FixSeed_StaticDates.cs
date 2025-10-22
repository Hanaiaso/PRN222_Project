using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN_Project.Migrations
{
    /// <inheritdoc />
    public partial class FixSeed_StaticDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Exams_EId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRanks_Exams_EId",
                table: "ExamRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRanks_Ranks_RaId",
                table: "ExamRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRanks_Students_SId",
                table: "ExamRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReceivers_Notifications_NtId",
                table: "NotificationReceivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Accounts_SenderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Submits_Exams_EId",
                table: "Submits");

            migrationBuilder.DropForeignKey(
                name: "FK_Submits_Students_SId",
                table: "Submits");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Subjects_SuId",
                table: "TeacherSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Teachers_TId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_SuId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_Submits_EId",
                table: "Submits");

            migrationBuilder.DropIndex(
                name: "IX_Submits_SId",
                table: "Submits");

            migrationBuilder.DropIndex(
                name: "IX_NotificationReceivers_NtId",
                table: "NotificationReceivers");

            migrationBuilder.DropIndex(
                name: "IX_ExamRanks_EId",
                table: "ExamRanks");

            migrationBuilder.DropIndex(
                name: "IX_ExamRanks_RaId",
                table: "ExamRanks");

            migrationBuilder.DropIndex(
                name: "IX_ExamRanks_SId",
                table: "ExamRanks");

            migrationBuilder.DropIndex(
                name: "IX_Answers_EId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "SubjectSuId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherTId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamEId",
                table: "Submits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentSId",
                table: "Submits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationNtId",
                table: "NotificationReceivers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamEId",
                table: "ExamRanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RankRaId",
                table: "ExamRanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentSId",
                table: "ExamRanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamEId",
                table: "Answers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Answers",
                keyColumn: "AwId",
                keyValue: 1,
                columns: new[] { "AnswerContent", "CreatedAt", "ExamEId" },
                values: new object[] { "[\r\n                        { \"Question\": \"What is 2 + 2?\", \"Options\": [\"3\", \"4\", \"5\", \"6\"], \"CorrectAnswer\": \"4\" },\r\n                        { \"Question\": \"What is 10 / 2?\", \"Options\": [\"2\", \"5\", \"10\", \"20\"], \"CorrectAnswer\": \"5\" }\r\n                    ]", new DateTime(2025, 1, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Answers",
                keyColumn: "AwId",
                keyValue: 2,
                columns: new[] { "AnswerContent", "CreatedAt", "ExamEId" },
                values: new object[] { "[\r\n                        { \"Question\": \"He ___ to school every day.\", \"Options\": [\"go\", \"goes\", \"going\", \"gone\"], \"CorrectAnswer\": \"goes\" },\r\n                        { \"Question\": \"Which word is a noun?\", \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"], \"CorrectAnswer\": \"happiness\" }\r\n                    ]", new DateTime(2025, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "ExamRanks",
                keyColumn: "ErId",
                keyValue: 1,
                columns: new[] { "ExamEId", "RankRaId", "StudentSId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "ExamRanks",
                keyColumn: "ErId",
                keyValue: 2,
                columns: new[] { "ExamEId", "RankRaId", "StudentSId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "EId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "EndTime", "ExamContent", "StartTime" },
                values: new object[] { new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "[\r\n                        { \"Question\": \"What is 2 + 2?\", \"Options\": [\"3\", \"4\", \"5\", \"6\"], \"CorrectAnswer\": \"4\" },\r\n                        { \"Question\": \"What is 10 / 2?\", \"Options\": [\"2\", \"5\", \"10\", \"20\"], \"CorrectAnswer\": \"5\" }\r\n                    ]", new DateTime(2025, 1, 2, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "EId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "EndTime", "ExamContent", "StartTime" },
                values: new object[] { new DateTime(2025, 1, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), "[\r\n                        { \"Question\": \"He ___ to school every day.\", \"Options\": [\"go\", \"goes\", \"going\", \"gone\"], \"CorrectAnswer\": \"goes\" },\r\n                        { \"Question\": \"Which word is a noun?\", \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"], \"CorrectAnswer\": \"happiness\" }\r\n                    ]", new DateTime(2025, 1, 4, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 1,
                column: "NotificationNtId",
                value: null);

            migrationBuilder.UpdateData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 2,
                column: "NotificationNtId",
                value: null);

            migrationBuilder.UpdateData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 3,
                column: "NotificationNtId",
                value: null);

            migrationBuilder.UpdateData(
                table: "NotificationReceivers",
                keyColumn: "NrId",
                keyValue: 4,
                column: "NotificationNtId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 1,
                columns: new[] { "Content", "SentTime" },
                values: new object[] { "We’re excited to have you here!", new DateTime(2025, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 2,
                columns: new[] { "Content", "SentTime" },
                values: new object[] { "Don't forget your math exam this Friday at 8:00 AM.", new DateTime(2025, 1, 7, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 3,
                columns: new[] { "Content", "SentTime" },
                values: new object[] { "LMS will be under maintenance this weekend.", new DateTime(2025, 1, 8, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Submits",
                keyColumn: "SbId",
                keyValue: 1,
                columns: new[] { "Comment", "Content", "ExamEId", "StudentSId", "SubmitTime" },
                values: new object[] { "Good performance", "[{\"QuestionIndex\":1,\"ChosenAnswer\":\"4\"},{\"QuestionIndex\":2,\"ChosenAnswer\":\"5\"}]", null, null, new DateTime(2025, 1, 4, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Submits",
                keyColumn: "SbId",
                keyValue: 2,
                columns: new[] { "Comment", "Content", "ExamEId", "StudentSId", "SubmitTime" },
                values: new object[] { "Needs to improve grammar", "[{\"QuestionIndex\":1,\"ChosenAnswer\":\"goes\"},{\"QuestionIndex\":2,\"ChosenAnswer\":\"run\"}]", null, null, new DateTime(2025, 1, 6, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "TeacherSubjects",
                keyColumn: "TsId",
                keyValue: 1,
                columns: new[] { "SubjectSuId", "TeacherTId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "TeacherSubjects",
                keyColumn: "TsId",
                keyValue: 2,
                columns: new[] { "SubjectSuId", "TeacherTId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SubjectSuId",
                table: "TeacherSubjects",
                column: "SubjectSuId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_TeacherTId",
                table: "TeacherSubjects",
                column: "TeacherTId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_ExamEId",
                table: "Submits",
                column: "ExamEId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_StudentSId",
                table: "Submits",
                column: "StudentSId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_NotificationNtId",
                table: "NotificationReceivers",
                column: "NotificationNtId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_ExamEId",
                table: "ExamRanks",
                column: "ExamEId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_RankRaId",
                table: "ExamRanks",
                column: "RankRaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_StudentSId",
                table: "ExamRanks",
                column: "StudentSId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_ExamEId",
                table: "Answers",
                column: "ExamEId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Exams_ExamEId",
                table: "Answers",
                column: "ExamEId",
                principalTable: "Exams",
                principalColumn: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRanks_Exams_ExamEId",
                table: "ExamRanks",
                column: "ExamEId",
                principalTable: "Exams",
                principalColumn: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRanks_Ranks_RankRaId",
                table: "ExamRanks",
                column: "RankRaId",
                principalTable: "Ranks",
                principalColumn: "RaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRanks_Students_StudentSId",
                table: "ExamRanks",
                column: "StudentSId",
                principalTable: "Students",
                principalColumn: "SId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReceivers_Notifications_NotificationNtId",
                table: "NotificationReceivers",
                column: "NotificationNtId",
                principalTable: "Notifications",
                principalColumn: "NtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Accounts_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "Accounts",
                principalColumn: "AId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submits_Exams_ExamEId",
                table: "Submits",
                column: "ExamEId",
                principalTable: "Exams",
                principalColumn: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submits_Students_StudentSId",
                table: "Submits",
                column: "StudentSId",
                principalTable: "Students",
                principalColumn: "SId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Subjects_SubjectSuId",
                table: "TeacherSubjects",
                column: "SubjectSuId",
                principalTable: "Subjects",
                principalColumn: "SuId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Teachers_TeacherTId",
                table: "TeacherSubjects",
                column: "TeacherTId",
                principalTable: "Teachers",
                principalColumn: "TId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Exams_ExamEId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRanks_Exams_ExamEId",
                table: "ExamRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRanks_Ranks_RankRaId",
                table: "ExamRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRanks_Students_StudentSId",
                table: "ExamRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReceivers_Notifications_NotificationNtId",
                table: "NotificationReceivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Accounts_SenderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Submits_Exams_ExamEId",
                table: "Submits");

            migrationBuilder.DropForeignKey(
                name: "FK_Submits_Students_StudentSId",
                table: "Submits");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Subjects_SubjectSuId",
                table: "TeacherSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Teachers_TeacherTId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_SubjectSuId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_TeacherTId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_Submits_ExamEId",
                table: "Submits");

            migrationBuilder.DropIndex(
                name: "IX_Submits_StudentSId",
                table: "Submits");

            migrationBuilder.DropIndex(
                name: "IX_NotificationReceivers_NotificationNtId",
                table: "NotificationReceivers");

            migrationBuilder.DropIndex(
                name: "IX_ExamRanks_ExamEId",
                table: "ExamRanks");

            migrationBuilder.DropIndex(
                name: "IX_ExamRanks_RankRaId",
                table: "ExamRanks");

            migrationBuilder.DropIndex(
                name: "IX_ExamRanks_StudentSId",
                table: "ExamRanks");

            migrationBuilder.DropIndex(
                name: "IX_Answers_ExamEId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "SubjectSuId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "TeacherTId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "ExamEId",
                table: "Submits");

            migrationBuilder.DropColumn(
                name: "StudentSId",
                table: "Submits");

            migrationBuilder.DropColumn(
                name: "NotificationNtId",
                table: "NotificationReceivers");

            migrationBuilder.DropColumn(
                name: "ExamEId",
                table: "ExamRanks");

            migrationBuilder.DropColumn(
                name: "RankRaId",
                table: "ExamRanks");

            migrationBuilder.DropColumn(
                name: "StudentSId",
                table: "ExamRanks");

            migrationBuilder.DropColumn(
                name: "ExamEId",
                table: "Answers");

            migrationBuilder.UpdateData(
                table: "Answers",
                keyColumn: "AwId",
                keyValue: 1,
                columns: new[] { "AnswerContent", "CreatedAt" },
                values: new object[] { "[\r\n                        {\r\n                            \"Question\": \"What is 2 + 2?\",\r\n                            \"Options\": [\"3\", \"4\", \"5\", \"6\"],\r\n                            \"CorrectAnswer\": \"4\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"What is 10 / 2?\",\r\n                            \"Options\": [\"2\", \"5\", \"10\", \"20\"],\r\n                            \"CorrectAnswer\": \"5\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 22, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(7780) });

            migrationBuilder.UpdateData(
                table: "Answers",
                keyColumn: "AwId",
                keyValue: 2,
                columns: new[] { "AnswerContent", "CreatedAt" },
                values: new object[] { "[\r\n                        {\r\n                            \"Question\": \"Choose the correct form: He ___ to school every day.\",\r\n                            \"Options\": [\"go\", \"goes\", \"going\", \"gone\"],\r\n                            \"CorrectAnswer\": \"goes\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"Which word is a noun?\",\r\n                            \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"],\r\n                            \"CorrectAnswer\": \"happiness\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 22, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(8281) });

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "EId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "EndTime", "ExamContent", "StartTime" },
                values: new object[] { new DateTime(2025, 10, 22, 22, 51, 36, 930, DateTimeKind.Local).AddTicks(3489), new DateTime(2025, 10, 24, 0, 51, 36, 931, DateTimeKind.Local).AddTicks(5305), "[\r\n                        {\r\n                            \"Question\": \"What is 2 + 2?\",\r\n                            \"Options\": [\"3\", \"4\", \"5\", \"6\"],\r\n                            \"CorrectAnswer\": \"4\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"What is 10 / 2?\",\r\n                            \"Options\": [\"2\", \"5\", \"10\", \"20\"],\r\n                            \"CorrectAnswer\": \"5\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 23, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(4978) });

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "EId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "EndTime", "ExamContent", "StartTime" },
                values: new object[] { new DateTime(2025, 10, 22, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(5799), new DateTime(2025, 10, 25, 23, 51, 36, 931, DateTimeKind.Local).AddTicks(5802), "[\r\n                        {\r\n                            \"Question\": \"Choose the correct form: He ___ to school every day.\",\r\n                            \"Options\": [\"go\", \"goes\", \"going\", \"gone\"],\r\n                            \"CorrectAnswer\": \"goes\"\r\n                        },\r\n                        {\r\n                            \"Question\": \"Which word is a noun?\",\r\n                            \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"],\r\n                            \"CorrectAnswer\": \"happiness\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 25, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(5800) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 1,
                columns: new[] { "Content", "SentTime" },
                values: new object[] { "We’re excited to have you here! Start exploring the exam system and check your profile for details.", new DateTime(2025, 10, 17, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(1727) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 2,
                columns: new[] { "Content", "SentTime" },
                values: new object[] { "Don't forget! The Math midterm exam will take place this Friday at 8:00 AM.", new DateTime(2025, 10, 20, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(1998) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NtId",
                keyValue: 3,
                columns: new[] { "Content", "SentTime" },
                values: new object[] { "The LMS system will be under maintenance this weekend. Please save your progress beforehand.", new DateTime(2025, 10, 21, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(2000) });

            migrationBuilder.UpdateData(
                table: "Submits",
                keyColumn: "SbId",
                keyValue: 1,
                columns: new[] { "Comment", "Content", "SubmitTime" },
                values: new object[] { "Good performance, minor mistakes.", "[\r\n                        {\r\n                            \"QuestionIndex\": 1,\r\n                            \"ChosenAnswer\": \"4\"\r\n                        },\r\n                        {\r\n                            \"QuestionIndex\": 2,\r\n                            \"ChosenAnswer\": \"5\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 21, 22, 51, 36, 931, DateTimeKind.Local).AddTicks(9782) });

            migrationBuilder.UpdateData(
                table: "Submits",
                keyColumn: "SbId",
                keyValue: 2,
                columns: new[] { "Comment", "Content", "SubmitTime" },
                values: new object[] { "Needs to improve grammar.", "[\r\n                        {\r\n                            \"QuestionIndex\": 1,\r\n                            \"ChosenAnswer\": \"goes\"\r\n                        },\r\n                        {\r\n                            \"QuestionIndex\": 2,\r\n                            \"ChosenAnswer\": \"run\"\r\n                        }\r\n                    ]", new DateTime(2025, 10, 20, 22, 51, 36, 932, DateTimeKind.Local).AddTicks(211) });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SuId",
                table: "TeacherSubjects",
                column: "SuId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_EId",
                table: "Submits",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_SId",
                table: "Submits",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_NtId",
                table: "NotificationReceivers",
                column: "NtId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Answers_EId",
                table: "Answers",
                column: "EId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Exams_EId",
                table: "Answers",
                column: "EId",
                principalTable: "Exams",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRanks_Exams_EId",
                table: "ExamRanks",
                column: "EId",
                principalTable: "Exams",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRanks_Ranks_RaId",
                table: "ExamRanks",
                column: "RaId",
                principalTable: "Ranks",
                principalColumn: "RaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRanks_Students_SId",
                table: "ExamRanks",
                column: "SId",
                principalTable: "Students",
                principalColumn: "SId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReceivers_Notifications_NtId",
                table: "NotificationReceivers",
                column: "NtId",
                principalTable: "Notifications",
                principalColumn: "NtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Accounts_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "Accounts",
                principalColumn: "AId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Submits_Exams_EId",
                table: "Submits",
                column: "EId",
                principalTable: "Exams",
                principalColumn: "EId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submits_Students_SId",
                table: "Submits",
                column: "SId",
                principalTable: "Students",
                principalColumn: "SId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Subjects_SuId",
                table: "TeacherSubjects",
                column: "SuId",
                principalTable: "Subjects",
                principalColumn: "SuId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Teachers_TId",
                table: "TeacherSubjects",
                column: "TId",
                principalTable: "Teachers",
                principalColumn: "TId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
