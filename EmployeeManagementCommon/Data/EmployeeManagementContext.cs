using EmployeeManagementCommon.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EmployeeManagementCommon.Data
{
    public class EmployeeManagementContext : DbContext
    {
        public EmployeeManagementContext (DbContextOptions<EmployeeManagementContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<LeaveDetails> LeaveDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<LeaveDetails>().ToTable("LeaveDetail");
        }
    }
}
