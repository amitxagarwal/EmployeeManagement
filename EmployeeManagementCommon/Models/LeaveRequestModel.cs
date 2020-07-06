using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementCommon.Models
{
    public class LeaveRequestModel
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReasonForLeave { get; set; }
    }
}
