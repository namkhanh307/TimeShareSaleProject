using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeShareProject.Models;

namespace TimeShareProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            ConfigureServices(builder.Services, builder.Configuration);

            // Build the application
            var app = builder.Build();

            // Add session
            builder.Services.AddDistributedMemoryCache();

            // Configure the application
            Configure(app, builder.Configuration);

            // Run the application 
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddControllersWithViews();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TimeShareProjectContext>(options => options.UseSqlServer(connectionString));

            services.AddHttpContextAccessor();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
       .AddCookie(options =>
       {
           options.LoginPath = "/Login/Login";
           options.LogoutPath = "/Login/Logout";
           options.AccessDeniedPath = "/Home/AccessDenied";
           options.ExpireTimeSpan = TimeSpan.FromSeconds(6*60);
           
           options.Events.OnRedirectToLogin = context =>
           {
               // Redirect the user to the home page after the cookie expires
               context.Response.Redirect("/");
               return Task.CompletedTask;
           };
       });
        }

        private static void Configure(WebApplication app, IConfiguration configuration)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Enable detailed errors in Development
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); // Use custom error handling in Production
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}

