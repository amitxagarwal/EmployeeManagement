using System;
using System.IO;
using System.Threading.Tasks;
using EmployeeManagementCommon;
using EmployeeManagementCommon.Models;
using EmployeeManagementCommon.Repository;
using EmployeeManagementServiceLayer;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace EmployeeManagementUnitTests
{
    public class LeaveTests
    {
        [Fact]
        public async Task AddLeaveSucceeds()
        {
            var leaveRepoMoq = new Mock<ILeaveRepository>();
            var employeeRepositoryMoq = new Mock<IEmployeeRepository>();
            var config = GetConifg();
            var emailHelperMoq = new Mock<IEmailHelper>();
            var leave = new LeaveRequestModel() { EmployeeId = 123, StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddDays(2).Date, ReasonForLeave = "testleave" };

            var leaveDetails = new LeaveDetails()
            {
                EmployeeId = leave.EmployeeId,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                ReasonForLeave = leave.ReasonForLeave,
                Status = LeaveStatus.Applied
            };

            leaveRepoMoq.Setup(x => x.AddLeaveAsync(It.IsAny<LeaveDetails>()))
                .Returns(Task.FromResult(new ResultOrHttpError<LeaveDetails, string>(leaveDetails)));

            var employee = new Employee()
            {
                EmailId = "amit.x.agarwal@gmail.com",
                EmployeeId = 123,
                FullName = "test",
                ManagerId = 1234
            };

            employeeRepositoryMoq.Setup(x => x.GetEmployeeAsync(It.IsAny<int>())).Returns(Task.FromResult(employee));

            var emailModel = new EmailModel(employee.EmailId, "Subject", "Message", true);

            emailHelperMoq.Setup(x => x.SendEmailAsync(It.IsAny<EmailModel>()))
                .Returns(Task.FromResult(new ResultOrHttpError<EmailModel, string>(emailModel)));

            var leaveService = new LeaveService(leaveRepoMoq.Object, emailHelperMoq.Object, employeeRepositoryMoq.Object, config);
            var leaveAddResult = await leaveService.AddLeave(leave);

            leaveAddResult.IsError.Should().BeFalse();
            leaveAddResult.Result.EmployeeId.Should().Be(leave.EmployeeId);
        }

        [Fact]
        public async Task AddLeaveEmailFailure()
        {
            var leaveRepoMoq = new Mock<ILeaveRepository>();
            var employeeRepositoryMoq = new Mock<IEmployeeRepository>();
            var config = GetConifg();
            var emailHelperMoq = new Mock<IEmailHelper>();
            var leave = new LeaveRequestModel() { EmployeeId = 123, StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddDays(2).Date, ReasonForLeave = "testleave" };

            var leaveDetails = new LeaveDetails()
            {
                EmployeeId = leave.EmployeeId,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                ReasonForLeave = leave.ReasonForLeave,
                Status = LeaveStatus.Applied
            };

            leaveRepoMoq.Setup(x => x.AddLeaveAsync(It.IsAny<LeaveDetails>()))
                .Returns(Task.FromResult(new ResultOrHttpError<LeaveDetails, string>(leaveDetails)));

            var employee = new Employee()
            {
                EmailId = "amit.x.agarwal@gmail.com",
                EmployeeId = 123,
                FullName = "test",
                ManagerId = 1234
            };

            employeeRepositoryMoq.Setup(x => x.GetEmployeeAsync(It.IsAny<int>())).Returns(Task.FromResult(employee));

            var emailModel = new EmailModel(employee.EmailId, "Subject", "Message", true);

            emailHelperMoq.Setup(x => x.SendEmailAsync(It.IsAny<EmailModel>()))
                .Returns(Task.FromResult(new ResultOrHttpError<EmailModel, string>("An Error Occured while sending the email")));

            var leaveService = new LeaveService(leaveRepoMoq.Object, emailHelperMoq.Object, employeeRepositoryMoq.Object, config);
            var leaveAddResult = await leaveService.AddLeave(leave);

            leaveAddResult.IsError.Should().BeTrue();
            leaveAddResult.Error.Should().Be("An Error has occured while sending the notification to the Manager:An Error Occured while sending the email");
        }

            private IConfiguration GetConifg()
        {
            var smtpConfig = new EmailConfiguration() { 
            From = "amit.x.agarwal@outlook.com",
            SmtpPassword = "test123",
            SmtpPort=587,
            SmtpServer= "smtp.office365.com",
            SmtpUsername="amit.x.agarwal@outlook.com",
            ImapServer= "outlook.office365.com",
            ImapPort=993,
            ImapUsername= "amit.x.agarwal@outlook.com",
            ImapPassword="test123"
            };
            
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(smtpConfig)));

            IConfiguration _config = new ConfigurationBuilder().AddJsonStream(memoryStream).Build();

            return _config;
        }
    }
}
