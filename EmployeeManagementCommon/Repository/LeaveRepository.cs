using EmployeeManagementCommon.Data;
using EmployeeManagementCommon.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementCommon.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly EmployeeManagementContext _context;

        public LeaveRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<ResultOrHttpError<LeaveDetails,string>> AddLeaveAsync(LeaveDetails leaveDetails)
        {
            try
            {
                var existingLeaveWithOverlappingDetails = await _context.LeaveDetails.FirstAsync(x => x.StartDate.Date == leaveDetails.StartDate.Date || x.EndDate.Date == leaveDetails.EndDate.Date);
                if(existingLeaveWithOverlappingDetails==null)
                    return new ResultOrHttpError<LeaveDetails, string>("There is already an existing leave with overlapping startdate or enddate or both for this employee");

                await _context.LeaveDetails.AddAsync(leaveDetails);
                await _context.SaveChangesAsync();

                return new ResultOrHttpError<LeaveDetails, string>(leaveDetails);
            }
            catch(Exception ex)
            {
                return new ResultOrHttpError<LeaveDetails, string>(ex.Message);
            }
        }

        public async Task<ResultOrHttpError<LeaveDetails, string>> UpdateLeaveAsync(int leaveId, LeaveStatus leaveStatus, string comment)
        {
            try
            {
                var leaveDetails = await  _context.LeaveDetails.FindAsync(leaveId);
                leaveDetails.Status = leaveStatus;
                leaveDetails.Comment = comment;

                _context.LeaveDetails.Update(leaveDetails);
                await _context.SaveChangesAsync();

                return new ResultOrHttpError<LeaveDetails, string>(leaveDetails);
            }
            catch (Exception ex)
            {
                return new ResultOrHttpError<LeaveDetails, string>(ex.Message);
            }
        }

        public async Task<ResultOrHttpError<LeaveDetails, string>> GetLeaveDetails(int leaveId)
        {
            try
            {
                var leaveDetails = await _context.LeaveDetails.FirstAsync(x => x.LeaveId == leaveId);

                return new ResultOrHttpError<LeaveDetails, string>(leaveDetails);
            }
            catch (Exception ex)
            {
                return new ResultOrHttpError<LeaveDetails, string>(ex.Message);
            }
        }
    }
}
