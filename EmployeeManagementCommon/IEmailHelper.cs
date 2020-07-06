using EmployeeManagementCommon.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementCommon
{
    public interface IEmailHelper
    {
        Task<ResultOrHttpError<EmailModel, string>> SendEmailAsync(EmailModel emailModel);
        Task<ResultOrHttpError<List<EmailMessage>, string>> GetAllLeaveApprovalRejectionEmails();

        Task DeleteAllApprovalRejectionEmailsAsync(List<EmailMessage> emails);
    }
}
