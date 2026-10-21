using CompleteWebProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Project.Models.Domains;
using Project.Models.Interfaces;
using Project.Models.Repository;

namespace CompleteWebProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Update to use ApplicationUser instead of IdentityUser
            builder.Services.AddDefaultIdentity<AspNetUsers>(options =>
            options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IRepository<Car>, GenericRepository<Car>>();
            builder.Services.AddScoped<IRepository<Booking>, GenericRepository<Booking>>();
            builder.Services.AddScoped<IRepository<AspNetUsers>, GenericRepository<AspNetUsers>>();
            builder.Services.AddScoped<IRepository<Review>, GenericRepository<Review>>();
            builder.Services.AddScoped<IRepository<Payment>, GenericRepository<Payment>>();

            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

            builder.Services.AddHttpContextAccessor();

            // Add session services
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24); // Set session timeout
                options.Cookie.HttpOnly = true; // Make the cookie HTTP only
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });

            // Add services to the container
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add the session middleware before authorization
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}