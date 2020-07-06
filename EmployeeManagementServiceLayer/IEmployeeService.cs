using EmployeeManagementCommon.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementServiceLayer
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeAsync(int empId);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
    }
}
