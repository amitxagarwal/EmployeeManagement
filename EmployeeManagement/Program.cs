using EmployeeManagementCommon.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().Build();
            var consoleMinLevel = config.GetValue("ConsoleLoggingMinLevel", defaultValue: LogEventLevel.Debug);
            var aspnetCoreLevel = config.GetValue("AspNetCoreLevel", defaultValue: LogEventLevel.Information);
            var seqServerUrl = config.GetValue("DiagnosticSeqServerUrl", defaultValue: "http://localhost:5341/");
            var seqApiKey = config.GetValue("DiagnosticSeqApiKey", defaultValue: "");
            var applicationName = typeof(Program).Assembly.GetName().Name;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft.AspNetCore", aspnetCoreLevel)
                .Enrich.WithProperty("Application", applicationName)
                .WriteTo.Console(restrictedToMinimumLevel: consoleMinLevel)
                .WriteTo.Seq(serverUrl: seqServerUrl, apiKey: seqApiKey, compact: true)
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<EmployeeManagementContext>();
                    DatabaseInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
