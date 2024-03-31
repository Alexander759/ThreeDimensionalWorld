using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NewProducts.SeedIdentity;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.DataAccess.Repository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Utility;
using ThreeDimensionalWorldWeb.Configuration;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

EmailSettings emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>()!;

builder.Services.AddScoped<IEmailSender>(sp =>
        new EmailSender(emailSettings.SmtpServer, emailSettings.Port, emailSettings.Mail, emailSettings.Password));

builder.Services.AddScoped<AppRolesAndUsersSeeder>();

builder.Services.AddHostedService<SeedDataService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

var app = builder.Build();

AppDbinitializer.SeedAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

CultureInfo culture = new CultureInfo("bg-BG");
Thread.CurrentThread.CurrentCulture = culture;
Thread.CurrentThread.CurrentUICulture = culture;

app.UseRequestLocalization("bg-BG");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Public}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
