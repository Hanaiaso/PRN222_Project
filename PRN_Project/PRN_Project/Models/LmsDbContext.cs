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
        public DbSet<Rank> Ranks { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<NotificationReceiver> NotificationReceivers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account: unique email
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // 1-1 relationships between Account and Student/Teacher/Admin
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

            // TeacherSubject: composite uniqueness (tId, suId)
            modelBuilder.Entity<TeacherSubject>()
                .HasIndex(ts => new { ts.TId, ts.SuId })
                .IsUnique();
            modelBuilder.Entity<TeacherSubject>()
                .HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(ts => ts.TId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherSubject>()
                .HasOne(ts => ts.Subject)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(ts => ts.SuId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exam defaults
            modelBuilder.Entity<Exam>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

           

            // Answer: relation to Exam
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Exam)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.EId)
                .OnDelete(DeleteBehavior.Cascade);


            // Submit -> Student, Exam
            modelBuilder.Entity<Submit>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Submits)
                .HasForeignKey(s => s.SId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submit>()
                .HasOne(s => s.Exam)
                .WithMany(e => e.Submits)
                .HasForeignKey(s => s.EId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submit>()
                .Property(s => s.SubmitTime)
                .HasDefaultValueSql("GETDATE()");

            // Rank -> Student, Exam
            modelBuilder.Entity<Rank>()
                .HasOne(r => r.Student)
                .WithMany(st => st.Ranks)
                .HasForeignKey(r => r.SId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rank>()
                .HasOne(r => r.Exam)
                .WithMany(e => e.Ranks)
                .HasForeignKey(r => r.EId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification & NotificationReceiver
            modelBuilder.Entity<Notification>()
                .Property(n => n.SentTime)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany(a => a.SentNotifications)
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NotificationReceiver>()
                .HasOne(nr => nr.Notification)
                .WithMany(n => n.Receivers)
                .HasForeignKey(nr => nr.NtId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationReceiver>()
                .HasOne(nr => nr.Receiver)
                .WithMany(a => a.NotificationReceivers)
                .HasForeignKey(nr => nr.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
