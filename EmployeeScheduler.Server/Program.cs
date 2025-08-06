using EmployeeScheduler.Server.UserAccount;
using EmployeeScheduler.Server.ShiftManagement;
using EmployeeScheduler.Server.Clock;
using EmployeeScheduler.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace EmployeeScheduler.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUserAccountManager, UserAccountManager>();
            builder.Services.AddScoped<IUserAccountEngine, UserAccountEngine>();
            builder.Services.AddScoped<IUserAccountAccessor, UserAccountAccessor>();

            builder.Services.AddScoped<IShiftManager, ShiftManager>();
            builder.Services.AddScoped<IShiftEngine, ShiftEngine>();
            builder.Services.AddScoped<IShiftAccessor, ShiftAccessor>();

            builder.Services.AddScoped<IClockManager, ClockManager>();
            builder.Services.AddScoped<IClockEngine, ClockEngine>();
            builder.Services.AddScoped<IClockAccessor, ClockAccessor>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddAuthentication("Cookies")
             .AddCookie("Cookies", options =>
             {
                 options.LoginPath = "";
                 options.AccessDeniedPath = "";

                 options.Events.OnRedirectToLogin = context =>
                 {
                     context.Response.StatusCode = 401;
                     return Task.CompletedTask;
                 };

                 options.Events.OnRedirectToAccessDenied = context =>
                 {
                     context.Response.StatusCode = 403;
                     return Task.CompletedTask;
                 };
             });


            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
