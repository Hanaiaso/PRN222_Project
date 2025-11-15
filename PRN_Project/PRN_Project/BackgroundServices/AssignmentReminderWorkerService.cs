using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PRN_Project.Services.Interfaces; // Namespace Service của bạn
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PRN_Project.BackgroundServices
{

    public class AssignmentReminderWorkerService : BackgroundService
    {
        private readonly ILogger<AssignmentReminderWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); 
        //private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(180);

        public AssignmentReminderWorkerService(
            ILogger<AssignmentReminderWorkerService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AssignmentReminderWorkerService đã khởi động. Sẽ kiểm tra mỗi {Interval} giờ.", _checkInterval.TotalHours);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker đang chạy lúc: {time}", DateTimeOffset.Now);

                    // Tối ưu #3: Tạo scope và gọi Service nghiệp vụ riêng
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reminderService = scope.ServiceProvider
                            .GetRequiredService<IAssignmentReminderService>();

                        // Ủy quyền toàn bộ logic cho Service
                        await reminderService.CheckAndSendRemindersAsync(stoppingToken);
                    }

                    _logger.LogInformation("Worker đã hoàn thành, chờ {Interval} giờ.", _checkInterval.TotalHours);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy AssignmentReminderWorkerService. Chi tiết: {Message}. StackTrace: {StackTrace}", 
                        ex.Message, ex.StackTrace);
                }

                // Đợi 1 giờ trước khi kiểm tra lại
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("AssignmentReminderWorkerService đã dừng.");
        }
    }
}