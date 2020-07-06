using Microsoft.Extensions.Configuration;
using System;

namespace LeaveStatusUpdateProcessor
{
    public static class Configuration
    {
        public static IConfigurationBuilder CreateBuilder()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder;
        }
    }
}
