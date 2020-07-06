using EmployeeManagementCommon;
using EmployeeManagementCommon.Models;
using EmployeeManagementCommon.Repository;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementServiceLayer
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly IEmployeeRepository _empRepo;
        private readonly string _from;
        private readonly IEmailHelper _emailHelper;

        public LeaveService(ILeaveRepository leaveRepo, IEmailHelper emailHelper, IEmployeeRepository empRepo, IConfiguration configuration)
        {
            _leaveRepo = leaveRepo;
            _emailHelper = emailHelper;
            _empRepo = empRepo;
            var smtpSection = configuration.GetSection("EmailConfiguration");
            if (smtpSection != null)
            {
                _from = smtpSection.GetSection("From").Value;
            }
        }

        public async Task<ResultOrHttpError<LeaveDetails, string>> AddLeave(LeaveRequestModel request)
        {
            Log.ForContext("EMPLOYEEID", request.EmployeeId)
                .Information("Adding the leave details");

            var leaveDetails = new LeaveDetails()
            {
                EmployeeId = request.EmployeeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReasonForLeave = request.ReasonForLeave,
                Status = LeaveStatus.Applied
            };

            var result = await _leaveRepo.AddLeaveAsync(leaveDetails);

            if (result.IsError)
            {
                Log.ForContext("EMPLOYEEID", request.EmployeeId)
                .Error($"Error while Adding the leave details:{result.Error}");

                return new ResultOrHttpError<LeaveDetails, string>("Error while creating the leave request:" + result.Error);
            }

            var employeeDetails = await _empRepo.GetEmployeeAsync(leaveDetails.EmployeeId);

            if (employeeDetails.ManagerId < 1)
            {
                Log.ForContext("EMPLOYEEID", request.EmployeeId)
                    .ForContext("LEAVEID", leaveDetails.LeaveId)
                .Error("Leave Request cant be created as Manager For this employee not found.");
                return new ResultOrHttpError<LeaveDetails, string>("Leave Request cant be created as Manager For this employee not found.");
            }

            var rejectionBodyHtml = $"mailto:{_from}?subject=Reject-request-REQ{result.Result.LeaveId}";
            var approvalBodyHtml = $"mailto:{_from}?subject=Approve-request-REQ{result.Result.LeaveId}";

            var body = string.Format(@"<html>< table >< tr >< td class=”button” ><a class=”link” href={0}>Approve</a></td><tb></td><tb></td><tb></td><tb></td><td class=”button”><a class=”link” href={1}>Reject</a></td></tr></table></html>", approvalBodyHtml, rejectionBodyHtml);
            var employeeManager = await _empRepo.GetEmployeeAsync(employeeDetails.ManagerId);
            var emailModel = new EmailModel(employeeManager.EmailId, $"Leave Approval Request REQ{result.Result.LeaveId}", body, true);
            var emailResult = await _emailHelper.SendEmailAsync(emailModel);

            if (emailResult.IsError)
            {
                Log.ForContext("EMPLOYEEID", request.EmployeeId)
                .Error($"An Error has occured while sending the notification to the Manager:{emailResult.Error}");
                return new ResultOrHttpError<LeaveDetails, string>("An Error has occured while sending the notification to the Manager:" + emailResult.Error);
            }

            return result;
        }

        public async Task<ResultOrHttpError<List<LeaveDetails>, string>> UpdateLeaveStatus()
        {
            var listOfEmails = await _emailHelper.GetAllLeaveApprovalRejectionEmails();
            List<LeaveDetails> lst = new List<LeaveDetails>();
            foreach (var email in listOfEmails.Result)
            {
                var leaveIdIndex = email.Subject.ToLower().IndexOf("-request-req");
                var lengthOfSub = email.Subject.Length;
                var comment = email.Content;

                var statusString = email.Subject.Substring(0, email.Subject.IndexOf("-"));
                var status = LeaveStatus.Rejected;
                if (statusString.ToLower() == "approve")
                    status = LeaveStatus.Approved;

                int.TryParse(email.Subject.Substring(leaveIdIndex + 12, lengthOfSub - (leaveIdIndex + 12)), out int leaveId);
                var leaveDetailsResult = await _leaveRepo.GetLeaveDetails(leaveId);
                if (leaveDetailsResult.IsError)
                {
                    Log.ForContext("LEAVEID", leaveId)
                        .Error($"Error Occured {leaveDetailsResult.Error}");
                    return new ResultOrHttpError<List<LeaveDetails>, string>("Error Occured" + leaveDetailsResult.Error);
                }
                var leaveDetails = leaveDetailsResult.Result;

                if (leaveDetails.Status != LeaveStatus.Applied)
                    continue;

                await _leaveRepo.UpdateLeaveAsync(leaveDetails.LeaveId, status, comment);
                lst.Add(leaveDetails);

                var employeeDetails = await _empRepo.GetEmployeeAsync(leaveDetails.EmployeeId);
                var body = $"You leave request with req id -{leaveDetails.LeaveId} is {status}";

                var emailModel = new EmailModel(employeeDetails.EmailId, $"Leave Request {status}", body, true);
                var emailResult = await _emailHelper.SendEmailAsync(emailModel);
                if (emailResult.IsError)
                {
                    Log.ForContext("EMPLOYEEID", leaveDetails.EmployeeId)
                        .ForContext("LEAVEID", leaveId)
                        .Error($"Error Occured {leaveDetailsResult.Error}");
                    return new ResultOrHttpError<List<LeaveDetails>, string>("An Error has occured while sending the notification to the Employee about the leave status update:" + emailResult.Error);
                }
            }

            await _emailHelper.DeleteAllApprovalRejectionEmailsAsync(listOfEmails.Result);

            return new ResultOrHttpError<List<LeaveDetails>, string>(lst);
        }
    }
}
