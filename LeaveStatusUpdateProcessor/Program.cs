using EmployeeManagementCommon;
using EmployeeManagementCommon.Data;
using EmployeeManagementCommon.Repository;
using EmployeeManagementServiceLayer;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace LeaveStatusUpdateProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = Configuration.CreateBuilder().Build();
            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(config)
                            .CreateLogger();
            try
            {                
                var builder = new HostBuilder()
                    .ConfigureWebJobs(b =>
                    {
                        b.AddAzureStorageCoreServices();
                        b.AddTimers();
                    })
                    .ConfigureAppConfiguration((context, c) =>
                    {
                        c.AddConfiguration(config);
                    })
                    .ConfigureLogging((context, b) =>
                    {
                        b.AddSerilog();                                                
                    })
                    .ConfigureServices((context, services) =>
                    {
                        AddServiceConfiguration(services, context.Configuration);
                        services.AddSingleton<IJobActivatorEx>(new LeaveStatusUpdateProcessorActivator(services.BuildServiceProvider()));
                    })
                    .UseConsoleLifetime();

                var host = builder.Build();
                Log.Information("Starting Leave Status Update Processor webjob host");
                using (host)
                {                    
                    await host.RunAsync().ConfigureAwait(false);
                }

                Log.Information("Shutting down Leave Status Update Processor webjob host");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Leave Status Update Processor webjob host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static void AddServiceConfiguration(IServiceCollection services, IConfiguration configuration)
        {            
            services.AddScoped<LeaveStatusUpdateProcessor>();
            services.AddScoped<IEmailHelper, EmailHelper>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.TryAddSingleton(configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddDbContext<EmployeeManagementContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("EmployeeManagementContext")));
        }
    }
}
