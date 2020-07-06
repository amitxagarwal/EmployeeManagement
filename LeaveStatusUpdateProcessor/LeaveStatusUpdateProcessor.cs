using EmployeeManagementServiceLayer;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LeaveStatusUpdateProcessor
{
    public class LeaveStatusUpdateProcessor
    {
        private readonly ILogger<LeaveStatusUpdateProcessor> _logger;
        private readonly ILeaveService _leaveService;

        public LeaveStatusUpdateProcessor(ILogger<LeaveStatusUpdateProcessor> logger, ILeaveService leaveService)
        {
            _logger = logger;
            _leaveService = leaveService;
        }

        public async Task TimerTriggerCleanupOldBlobsAsync(
            [TimerTrigger("0 */5 * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            if (timerInfo.IsPastDue)
            {
                _logger.LogInformation("Leave status update timer is running late");
            }

            var listLeaveDetails = await _leaveService.UpdateLeaveStatus();

            if (listLeaveDetails.IsError)
            {
                _logger.LogError("An error has occured:{@Error}", listLeaveDetails.Error);
            }
            else
            {
                _logger.LogInformation("Leaves updated with the updated status");
            }
        }
    }
}
