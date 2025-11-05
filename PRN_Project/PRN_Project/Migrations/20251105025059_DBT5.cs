using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PRN_Project.Migrations
{
    /// <inheritdoc />
    public partial class DBT5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AId);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    RaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MinScore = table.Column<float>(type: "real", nullable: true),
                    MaxScore = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.RaId);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SuId);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AId = table.Column<int>(type: "int", nullable: false),
                    AdName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdId);
                    table.ForeignKey(
                        name: "FK_Admins_Accounts_AId",
                        column: x => x.AId,
                        principalTable: "Accounts",
                        principalColumn: "AId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NtId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    SenderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NtId);
                    table.ForeignKey(
                        name: "FK_Notifications_Accounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Accounts",
                        principalColumn: "AId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    SId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AId = table.Column<int>(type: "int", nullable: false),
                    SName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.SId);
                    table.ForeignKey(
                        name: "FK_Students_Accounts_AId",
                        column: x => x.AId,
                        principalTable: "Accounts",
                        principalColumn: "AId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    TId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AId = table.Column<int>(type: "int", nullable: false),
                    TName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.TId);
                    table.ForeignKey(
                        name: "FK_Teachers_Accounts_AId",
                        column: x => x.AId,
                        principalTable: "Accounts",
                        principalColumn: "AId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    EId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuId = table.Column<int>(type: "int", nullable: true),
                    EName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExamContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.EId);
                    table.ForeignKey(
                        name: "FK_Exams_Subjects_SuId",
                        column: x => x.SuId,
                        principalTable: "Subjects",
                        principalColumn: "SuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationReceivers",
                columns: table => new
                {
                    NrId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NtId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationReceivers", x => x.NrId);
                    table.ForeignKey(
                        name: "FK_NotificationReceivers_Accounts_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Accounts",
                        principalColumn: "AId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationReceivers_Notifications_NtId",
                        column: x => x.NtId,
                        principalTable: "Notifications",
                        principalColumn: "NtId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjects",
                columns: table => new
                {
                    TsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TId = table.Column<int>(type: "int", nullable: false),
                    SuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjects", x => x.TsId);
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Subjects_SuId",
                        column: x => x.SuId,
                        principalTable: "Subjects",
                        principalColumn: "SuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Teachers_TId",
                        column: x => x.TId,
                        principalTable: "Teachers",
                        principalColumn: "TId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AwId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EId = table.Column<int>(type: "int", nullable: false),
                    AnswerContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.AwId);
                    table.ForeignKey(
                        name: "FK_Answers_Exams_EId",
                        column: x => x.EId,
                        principalTable: "Exams",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submits",
                columns: table => new
                {
                    SbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SId = table.Column<int>(type: "int", nullable: false),
                    EId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submits", x => x.SbId);
                    table.ForeignKey(
                        name: "FK_Submits_Exams_EId",
                        column: x => x.EId,
                        principalTable: "Exams",
                        principalColumn: "EId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submits_Students_SId",
                        column: x => x.SId,
                        principalTable: "Students",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamRanks",
                columns: table => new
                {
                    ErId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuId = table.Column<int>(type: "int", nullable: false),
                    RaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamRanks", x => x.ErId);
                    table.ForeignKey(
                        name: "FK_ExamRanks_Ranks_RaId",
                        column: x => x.RaId,
                        principalTable: "Ranks",
                        principalColumn: "RaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamRanks_Submits_SuId",
                        column: x => x.SuId,
                        principalTable: "Submits",
                        principalColumn: "SbId",
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
                table: "Exams",
                columns: new[] { "EId", "CreatedAt", "EName", "EndTime", "ExamContent", "StartTime", "Status", "SuId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Math Midterm Exam", new DateTime(2025, 1, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "[\r\n                        { \"Question\": \"What is 2 + 2?\", \"Options\": [\"3\", \"4\", \"5\", \"6\"], \"CorrectAnswer\": \"4\" },\r\n                        { \"Question\": \"What is 10 / 2?\", \"Options\": [\"2\", \"5\", \"10\", \"20\"], \"CorrectAnswer\": \"5\" }\r\n                    ]", new DateTime(2025, 1, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 2, new DateTime(2025, 1, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), "English Grammar Test", new DateTime(2025, 1, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), "[\r\n                        { \"Question\": \"He ___ to school every day.\", \"Options\": [\"go\", \"goes\", \"going\", \"gone\"], \"CorrectAnswer\": \"goes\" },\r\n                        { \"Question\": \"Which word is a noun?\", \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"], \"CorrectAnswer\": \"happiness\" }\r\n                    ]", new DateTime(2025, 1, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), true, 2 }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NtId", "Content", "SenderId", "SentTime", "Title" },
                values: new object[,]
                {
                    { 1, "We’re excited to have you here!", 1, new DateTime(2025, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Welcome to the LMS System" },
                    { 2, "Don't forget your math exam this Friday at 8:00 AM.", 2, new DateTime(2025, 1, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), "Upcoming Math Exam Reminder" },
                    { 3, "LMS will be under maintenance this weekend.", 1, new DateTime(2025, 1, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), "System Maintenance" }
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
                table: "Answers",
                columns: new[] { "AwId", "AnswerContent", "CreatedAt", "EId", "Status" },
                values: new object[,]
                {
                    { 1, "[\r\n                        { \"Question\": \"What is 2 + 2?\", \"Options\": [\"3\", \"4\", \"5\", \"6\"], \"CorrectAnswer\": \"4\" },\r\n                        { \"Question\": \"What is 10 / 2?\", \"Options\": [\"2\", \"5\", \"10\", \"20\"], \"CorrectAnswer\": \"5\" }\r\n                    ]", new DateTime(2025, 1, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, true },
                    { 2, "[\r\n                        { \"Question\": \"He ___ to school every day.\", \"Options\": [\"go\", \"goes\", \"going\", \"gone\"], \"CorrectAnswer\": \"goes\" },\r\n                        { \"Question\": \"Which word is a noun?\", \"Options\": [\"run\", \"beautiful\", \"happiness\", \"quickly\"], \"CorrectAnswer\": \"happiness\" }\r\n                    ]", new DateTime(2025, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), 2, true }
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
                    { 1, "Good performance", "[{\"QuestionIndex\":1,\"ChosenAnswer\":\"4\"},{\"QuestionIndex\":2,\"ChosenAnswer\":\"5\"}]", 1, 1, 9.0, new DateTime(2025, 1, 4, 8, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Needs to improve grammar", "[{\"QuestionIndex\":1,\"ChosenAnswer\":\"goes\"},{\"QuestionIndex\":2,\"ChosenAnswer\":\"run\"}]", 2, 2, 7.5, new DateTime(2025, 1, 6, 8, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TeacherSubjects",
                columns: new[] { "TsId", "SuId", "TId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "ExamRanks",
                columns: new[] { "ErId", "RaId", "SuId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AId",
                table: "Admins",
                column: "AId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_EId",
                table: "Answers",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_RaId",
                table: "ExamRanks",
                column: "RaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRanks_SuId_RaId",
                table: "ExamRanks",
                columns: new[] { "SuId", "RaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SuId",
                table: "Exams",
                column: "SuId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_NtId",
                table: "NotificationReceivers",
                column: "NtId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReceivers_ReceiverId",
                table: "NotificationReceivers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AId",
                table: "Students",
                column: "AId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submits_EId",
                table: "Submits",
                column: "EId");

            migrationBuilder.CreateIndex(
                name: "IX_Submits_SId",
                table: "Submits",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_AId",
                table: "Teachers",
                column: "AId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SuId",
                table: "TeacherSubjects",
                column: "SuId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_TId_SuId",
                table: "TeacherSubjects",
                columns: new[] { "TId", "SuId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "ExamRanks");

            migrationBuilder.DropTable(
                name: "NotificationReceivers");

            migrationBuilder.DropTable(
                name: "TeacherSubjects");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Submits");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
