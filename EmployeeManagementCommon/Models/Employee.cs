using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementCommon.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string FullName { get; set; }
        
        public string EmailId { get; set; }

        public int ManagerId { get; set; } = -9999;

        public List<LeaveDetails> LeaveList { get; set; }
    }
}
