using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeManagementCommon.Models
{
    public enum LeaveStatus
    {
        Applied,
        Approved,
        Rejected
    }
    public class LeaveDetails
    {
        [Key]
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReasonForLeave { get; set; }
        public string Comment { get; set; }
        public LeaveStatus Status { get; set; }
    }
}
