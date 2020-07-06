using EmployeeManagementCommon.Models;
using EmployeeManagementCommon.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementServiceLayer
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _empRepo;
        public EmployeeService(IEmployeeRepository empRepo)
        {
            _empRepo = empRepo;
        }
        public async Task<Employee> GetEmployeeAsync(int empId)
        {
            Log.ForContext("EMPLOYEEID", empId)
                .Information("Getting the details of employee");
            return await _empRepo.GetEmployeeAsync(empId);
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            return await _empRepo.UpdateEmployeeAsync(employee);
        }
    }
}
