using EmployeeManagementCommon;
using EmployeeManagementCommon.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementServiceLayer
{
    public interface ILeaveService
    {
        Task<ResultOrHttpError<LeaveDetails, string>> AddLeave(LeaveRequestModel request);
        Task<ResultOrHttpError<List<LeaveDetails>, string>> UpdateLeaveStatus();
    }
}
