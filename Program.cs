
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using TimeShareProject.Models;
using TimeShareProject.Services;
using TimeShareWebProject.Services;

namespace TimeShareProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IVnPayService, VnPayService>();


            builder.Services.AddSingleton(x => new PaypalClient(
                builder.Configuration["PaypalOPtions:AppId"],
                builder.Configuration["PaypalOPtions:AppSecret"],
                builder.Configuration["PaypalOPtionsMode"]
                )
            );

            builder.Services.AddHostedService<DateCheckerService>();
            builder.Services.AddScoped<IModelService, ModelService>();

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
            //services.AddDbContext<_4restContext>(options =>
            //{
            //    options.UseSqlServer(connectionString, sqlServerOptions =>
            //    {
            //        sqlServerOptions.EnableRetryOnFailure();
            //    });
            //});
      services.AddDbContext<_4restContext>(options => options.UseSqlServer(connectionString));

            services.AddHttpContextAccessor();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
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
           options.ExpireTimeSpan = TimeSpan.FromSeconds(6 * 60 * 60);

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
                app.UseStatusCodePagesWithRedirects("/Error/ErrorNotFound");
                app.UseHsts();
            }
            app.UseStatusCodePagesWithRedirects("/Error/ErrorNotFound");
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

