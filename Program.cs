using Project1.Database;
using Project1.Processors;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Configuration;
using Microsoft.Extensions.Hosting.WindowsServices;
using Project1.Services;

namespace Project1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new WebApplicationOptions() { 
                ContentRootPath = AppContext.BaseDirectory
            };
            var builder = WebApplication.CreateBuilder(options);
            //builder.Host.UseWindowsService().UseContentRoot();

            builder.Services.AddCors(options => 
                options.AddDefaultPolicy(
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Logging.AddSerilog();
            builder.Services.AddControllersWithViews();
            builder.Services.AddHostedService<ProcessorBackgroundWorker>();
            builder.Services.AddSerilog();
            builder.Services.AddCors();
            builder.Services.AddSerilog();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            //app.Urls.Add("https://localhost:7251/");
            app.MapControllerRoute(
                name: "default",
                pattern: "api/{controller}/{action=Index}/{id?}");
            
            app.Run();
        }
    }
}

