using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementCommon.Models
{
    public class EmailModel
    {
        public string To { get; }

        public string Subject { get; }

        public string Message { get; }

        public bool IsBodyHtml { get; }

        public EmailModel(string to, string subject, string message, bool isBodyHtml)
        {
            To = to;
            Subject = subject;
            Message = message;
            IsBodyHtml = isBodyHtml;
        }        
    }
}
