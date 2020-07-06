using EmployeeManagementCommon.Models;
using System.Linq;

namespace EmployeeManagementCommon.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(EmployeeManagementContext context)
        {
            context.Database.EnsureCreated();
            
            if (context.Employee.Any())
            {
                return;
            }

            var manager = new Employee { FullName = "Manager of Amit Agarwal", EmailId = "amitagarwal2704@gmail.com" };

            context.Employee.Add(manager);
            context.SaveChanges();

            var employee = new Employee { FullName = "Amit Agarwal", EmailId = "amit.x.agarwal@outlook.com", ManagerId = manager.EmployeeId };
            
            context.Employee.Add(employee);
            context.SaveChanges();            
        }
    }
}
