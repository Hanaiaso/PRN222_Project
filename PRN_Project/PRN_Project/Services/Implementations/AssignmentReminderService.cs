using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN_Project.Models;
using PRN_Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PRN_Project.Services.Implementations
{
    public class AssignmentReminderService : IAssignmentReminderService
    {
        private readonly LmsDbContext _dbContext; // Tên DbContext của bạn
        private readonly IEmailService _emailService;
        private readonly ILogger<AssignmentReminderService> _logger;
        private readonly IConfiguration _config;

        public AssignmentReminderService(
            LmsDbContext dbContext,
            IEmailService emailService,
            ILogger<AssignmentReminderService> logger,
            IConfiguration config)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _logger = logger;
            _config = config;
        }

        public async Task CheckAndSendRemindersAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var warningEndTime = now.AddHours(24);

            // Tìm các assignment có DueDate trong khoảng [bây giờ, 24 giờ tới]
            var assignmentsToNotify = await _dbContext.Posts
                .Where(p => p.PostType == "Assignment"
                    && p.DueDate.HasValue
                    && p.DueDate.Value > now
                    && p.DueDate.Value <= warningEndTime
                    && p.Classroom != null)
                .Include(p => p.Classroom!)
                    .ThenInclude(c => c.Members!)
                        .ThenInclude(m => m.Student!)
                            .ThenInclude(s => s.Account)
                .ToListAsync(cancellationToken);

            if (!assignmentsToNotify.Any())
                return;

            int totalEmailsSent = 0;
            var senderEmail = _config["EmailSettings:SenderEmail"] ?? "";

            foreach (var assignment in assignmentsToNotify)
            {
                if (assignment.Classroom?.Members == null || !assignment.Classroom.Members.Any())
                    continue;

                // BƯỚC 1: Lấy danh sách học sinh chưa nộp bài
                var allStudentIds = assignment.Classroom.Members
                    .Where(m => m.Student != null && m.Student.Account != null)
                    .Select(m => m.Student!.SId)
                    .ToList();

                if (!allStudentIds.Any()) continue;

                var submittedStudentIds = await _dbContext.AssignmentSubmissions
                    .Where(s => s.PostId == assignment.PostId && allStudentIds.Contains(s.Sid))
                    .Select(s => s.Sid)
                    .ToListAsync(cancellationToken);

                var studentsNotSubmitted = assignment.Classroom.Members
                    .Where(m => m.Student != null
                        && m.Student.Account != null
                        && !submittedStudentIds.Contains(m.Student.SId))
                    .Select(m => m.Student!)
                    .ToList();

                if (!studentsNotSubmitted.Any()) continue;

                // BƯỚC 2: Loại bỏ những học sinh đã được gửi email
                var studentIdsNotSubmitted = studentsNotSubmitted.Select(s => s.SId).ToList();
                var notifiedStudentIds = await _dbContext.AssignmentEmailNotifications
                    .Where(n => n.PostId == assignment.PostId
                        && n.NotificationType == "OneDayReminder"
                        && studentIdsNotSubmitted.Contains(n.StudentId))
                    .Select(n => n.StudentId)
                    .ToListAsync(cancellationToken);

                var studentsToNotify = studentsNotSubmitted
                    .Where(s => !notifiedStudentIds.Contains(s.SId))
                    .ToList();

                if (!studentsToNotify.Any()) continue;

                // BƯỚC 3: Gửi email
                var notificationsToAdd = new List<AssignmentEmailNotification>();

                foreach (var student in studentsToNotify)
                {
                    if (student.Account == null) continue;
                    
                    // Bỏ qua nếu email học sinh trùng với sender email
                    if (!string.IsNullOrEmpty(senderEmail) && 
                        student.Account.Email.Equals(senderEmail, StringComparison.OrdinalIgnoreCase))
                        continue;

                    try
                    {
                        _emailService.SendAssignmentReminderEmail(
                            student.Account.Email,
                            student.SName,
                            assignment.Title,
                            assignment.DueDate!.Value
                        );

                        notificationsToAdd.Add(new AssignmentEmailNotification
                        {
                            PostId = assignment.PostId,
                            StudentId = student.SId,
                            NotificationType = "OneDayReminder",
                            SentAt = now
                        });

                        totalEmailsSent++;
                    }
                    catch
                    {
                        // Bỏ qua lỗi, tiếp tục với học sinh tiếp theo
                    }
                }

                if (notificationsToAdd.Any())
                {
                    await _dbContext.AssignmentEmailNotifications.AddRangeAsync(notificationsToAdd, cancellationToken);
                }
            }

            if (totalEmailsSent > 0)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}