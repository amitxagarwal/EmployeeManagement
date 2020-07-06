using EmployeeManagementServiceLayer;
using EmployeeManagementCommon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace EmployeeManagement
{
    [Route("api/employeemanagement")]
    [Produces("application/json", "text/json")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _empService;
        private readonly ILeaveService _leaveService;
        public EmployeesController(IEmployeeService empService, ILeaveService leaveService)
        {
            _empService = empService;
            _leaveService = leaveService;
        }
        
        // GET: api/Employees/5
        [HttpGet("{empId}")]
        public async Task<ActionResult<Employee>> GetEmployee(int empId)
        {
            var employee = await _empService.GetEmployeeAsync(empId);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/Employees
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Employee>> PostLeave(LeaveRequestModel request)
        {
            var result = (await _leaveService.AddLeave(request));
            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error);
            }
            else
            {
                return Ok(result.Result);
            }
        }        
    }
}
