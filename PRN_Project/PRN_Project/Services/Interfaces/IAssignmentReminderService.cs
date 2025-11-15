using System.Threading;
using System.Threading.Tasks;

namespace PRN_Project.Services.Interfaces
{
    public interface IAssignmentReminderService
    {
        Task CheckAndSendRemindersAsync(CancellationToken cancellationToken);
    }
}