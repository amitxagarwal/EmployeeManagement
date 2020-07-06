using EmployeeManagementCommon.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementCommon.Repository
{
    public interface ILeaveRepository
    {        
        Task<ResultOrHttpError<LeaveDetails, string>> AddLeaveAsync(LeaveDetails leaveDetails);
        Task<ResultOrHttpError<LeaveDetails, string>> UpdateLeaveAsync(int leaveId, LeaveStatus leaveStatus, string comment);

        Task<ResultOrHttpError<LeaveDetails, string>> GetLeaveDetails(int leaveId);
    }
}
