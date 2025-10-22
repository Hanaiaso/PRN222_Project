using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Models
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<TeacherSubject> TeacherSubjects { get; set; } = null!;
        public DbSet<Exam> Exams { get; set; } = null!;
        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<Submit> Submits { get; set; } = null!;
        public DbSet<ExamRank> ExamRanks { get; set; } = null!;
        public DbSet<Rank> Ranks { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<NotificationReceiver> NotificationReceivers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Cấu hình các quan hệ ===

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.AId)
                .IsUnique();
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(s => s.AId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.AId)
                .IsUnique();
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Account)
                .WithOne(a => a.Teacher)
                .HasForeignKey<Teacher>(t => t.AId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.AId)
                .IsUnique();
            modelBuilder.Entity<Admin>()
                .HasOne(ad => ad.Account)
                .WithOne(a => a.Admin)
                .HasForeignKey<Admin>(ad => ad.AId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherSubject>()
                .HasIndex(ts => new { ts.TId, ts.SuId })
                .IsUnique();

            modelBuilder.Entity<Exam>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Submit>()
                .Property(s => s.SubmitTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Notification>()
                .Property(n => n.SentTime)
                .HasDefaultValueSql("GETDATE()");

            // === SEED DỮ LIỆU TĨNH ===

            modelBuilder.Entity<Account>().HasData(
                new Account { AId = 1, Email = "admin@example.com", Password = "Admin@123", Role = RoleType.Admin, Status = true },
                new Account { AId = 2, Email = "teacher@example.com", Password = "Teacher@123", Role = RoleType.Teacher, Status = true },
                new Account { AId = 3, Email = "student@example.com", Password = "Student@123", Role = RoleType.Student, Status = true },
                new Account { AId = 4, Email = "student2@example.com", Password = "Student@123", Role = RoleType.Student, Status = true }
            );

            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdId = 1, AId = 1, AdName = "Super Admin" }
            );

            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { TId = 1, AId = 2, TName = "Nguyen Van B", Qualification = "Master of Mathematics" }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { SId = 1, AId = 3, SName = "Tran Thi C", Gender = "Female", Dob = new DateTime(2008, 5, 12) },
                new Student { SId = 2, AId = 4, SName = "Le Van D", Gender = "Male", Dob = new DateTime(2008, 6, 12) }
            );

            modelBuilder.Entity<Subject>().HasData(
                new Subject { SuId = 1, SuName = "Mathematics" },
                new Subject { SuId = 2, SuName = "English" }
            );

            modelBuilder.Entity<TeacherSubject>().HasData(
                new TeacherSubject { TsId = 1, TId = 1, SuId = 1 },
                new TeacherSubject { TsId = 2, TId = 1, SuId = 2 }
            );

            // Dùng ngày giờ cố định thay vì DateTime.Now
            var fixedDate = new DateTime(2025, 1, 1, 8, 0, 0);
            var fixedDate2 = new DateTime(2025, 1, 3, 8, 0, 0);

            modelBuilder.Entity<Exam>().HasData(
                new Exam
                {
                    EId = 1,
                    EName = "Math Midterm Exam",
                    SuId = 1,
                    CreatedAt = fixedDate,
                    StartTime = fixedDate.AddDays(1),
                    EndTime = fixedDate.AddDays(1).AddHours(2),
                    Status = true,
                    ExamContent = @"[
                        { ""Question"": ""What is 2 + 2?"", ""Options"": [""3"", ""4"", ""5"", ""6""], ""CorrectAnswer"": ""4"" },
                        { ""Question"": ""What is 10 / 2?"", ""Options"": [""2"", ""5"", ""10"", ""20""], ""CorrectAnswer"": ""5"" }
                    ]"
                },
                new Exam
                {
                    EId = 2,
                    EName = "English Grammar Test",
                    SuId = 2,
                    CreatedAt = fixedDate2,
                    StartTime = fixedDate2.AddDays(1),
                    EndTime = fixedDate2.AddDays(1).AddHours(1),
                    Status = true,
                    ExamContent = @"[
                        { ""Question"": ""He ___ to school every day."", ""Options"": [""go"", ""goes"", ""going"", ""gone""], ""CorrectAnswer"": ""goes"" },
                        { ""Question"": ""Which word is a noun?"", ""Options"": [""run"", ""beautiful"", ""happiness"", ""quickly""], ""CorrectAnswer"": ""happiness"" }
                    ]"
                }
            );

            modelBuilder.Entity<Rank>().HasData(
                new Rank { RaId = 1, RankName = "A", MaxScore = 10.0f, MinScore = 8.5f },
                new Rank { RaId = 2, RankName = "B", MaxScore = 8.4f, MinScore = 6.5f },
                new Rank { RaId = 3, RankName = "C", MaxScore = 6.4f, MinScore = 4.5f },
                new Rank { RaId = 4, RankName = "D", MaxScore = 4.4f, MinScore = 0.0f }
            );

            modelBuilder.Entity<Answer>().HasData(
                new Answer
                {
                    AwId = 1,
                    EId = 1,
                    CreatedAt = fixedDate.AddDays(2),
                    Status = true,
                    AnswerContent = @"[
                        { ""Question"": ""What is 2 + 2?"", ""Options"": [""3"", ""4"", ""5"", ""6""], ""CorrectAnswer"": ""4"" },
                        { ""Question"": ""What is 10 / 2?"", ""Options"": [""2"", ""5"", ""10"", ""20""], ""CorrectAnswer"": ""5"" }
                    ]"
                },
                new Answer
                {
                    AwId = 2,
                    EId = 2,
                    CreatedAt = fixedDate2.AddDays(2),
                    Status = true,
                    AnswerContent = @"[
                        { ""Question"": ""He ___ to school every day."", ""Options"": [""go"", ""goes"", ""going"", ""gone""], ""CorrectAnswer"": ""goes"" },
                        { ""Question"": ""Which word is a noun?"", ""Options"": [""run"", ""beautiful"", ""happiness"", ""quickly""], ""CorrectAnswer"": ""happiness"" }
                    ]"
                }
            );

            modelBuilder.Entity<Submit>().HasData(
                new Submit
                {
                    SbId = 1,
                    SId = 1,
                    EId = 1,
                    Score = 9.0,
                    SubmitTime = fixedDate.AddDays(3),
                    Comment = "Good performance",
                    Content = @"[{""QuestionIndex"":1,""ChosenAnswer"":""4""},{""QuestionIndex"":2,""ChosenAnswer"":""5""}]"
                },
                new Submit
                {
                    SbId = 2,
                    SId = 2,
                    EId = 2,
                    Score = 7.5,
                    SubmitTime = fixedDate2.AddDays(3),
                    Comment = "Needs to improve grammar",
                    Content = @"[{""QuestionIndex"":1,""ChosenAnswer"":""goes""},{""QuestionIndex"":2,""ChosenAnswer"":""run""}]"
                }
            );

            modelBuilder.Entity<ExamRank>().HasData(
                new ExamRank { ErId = 1, SId = 1, EId = 1, RaId = 1 },
                new ExamRank { ErId = 2, SId = 2, EId = 2, RaId = 2 }
            );

            // ===== Notification + Receiver =====
            modelBuilder.Entity<Notification>().HasData(
                new Notification
                {
                    NtId = 1,
                    Title = "Welcome to the LMS System",
                    Content = "We’re excited to have you here!",
                    SentTime = new DateTime(2025, 1, 5, 8, 0, 0),
                    SenderId = 1
                },
                new Notification
                {
                    NtId = 2,
                    Title = "Upcoming Math Exam Reminder",
                    Content = "Don't forget your math exam this Friday at 8:00 AM.",
                    SentTime = new DateTime(2025, 1, 7, 8, 0, 0),
                    SenderId = 2
                },
                new Notification
                {
                    NtId = 3,
                    Title = "System Maintenance",
                    Content = "LMS will be under maintenance this weekend.",
                    SentTime = new DateTime(2025, 1, 8, 8, 0, 0),
                    SenderId = 1
                }
            );

            modelBuilder.Entity<NotificationReceiver>().HasData(
                new NotificationReceiver { NrId = 1, NtId = 1, ReceiverId = 3, IsRead = true },
                new NotificationReceiver { NrId = 2, NtId = 1, ReceiverId = 4, IsRead = false },
                new NotificationReceiver { NrId = 3, NtId = 2, ReceiverId = 3, IsRead = false },
                new NotificationReceiver { NrId = 4, NtId = 3, ReceiverId = 2, IsRead = true }
            );
        }
    }
}
