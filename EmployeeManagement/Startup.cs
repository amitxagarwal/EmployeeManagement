using EmployeeManagementCommon;
using EmployeeManagementCommon.Data;
using EmployeeManagementCommon.Repository;
using EmployeeManagementServiceLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;

namespace EmployeeManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<IEmailHelper, EmailHelper>();
            services.TryAddSingleton(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();
                // Add a SwaggerDoc for v1
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Kmd Momentum External Api",
                        Description = "Kmd Momentum External Api - v1",
                    });
                
                c.EnableAnnotations();

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFile = Path.Combine(baseDirectory, xmlFileName);

                c.IncludeXmlComments(xmlFile);                
            });

            services.AddDbContext<EmployeeManagementContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("EmployeeManagementContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API.");
                c.DefaultModelRendering(ModelRendering.Model);
                c.DisplayRequestDuration();
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
