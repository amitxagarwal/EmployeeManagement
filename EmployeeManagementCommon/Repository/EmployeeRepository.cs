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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeManagementContext _context;

        public EmployeeRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetEmployeeAsync(int empId)
        {
            return await _context.Employee.FindAsync(empId);
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            _context.Employee.Update(employee);
            await _context.SaveChangesAsync();
            return await _context.Employee.FindAsync(employee.EmployeeId);
        }
        
    }
}
