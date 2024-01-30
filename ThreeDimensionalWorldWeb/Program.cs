using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NewProducts.SeedIdentity;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Utility;
using ThreeDimensionalWorldWeb.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();*/

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<AppRolesAndUsersSeeder>();

builder.Services.AddHostedService<SeedDataService>();

//May be
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

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


var fileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "3dModels"));
var requestPath = "/3dModels";

/*// Enable displaying browser links.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = fileProvider,
    Reques*//*tPath = requestPath
});
*/
FileServerOptions options = new FileServerOptions();
options.DefaultFilesOptions.DefaultFileNames.Clear();
options.DefaultFilesOptions.DefaultFileNames.Add("3dModels/3d.glb");
options.EnableDirectoryBrowsing = true;
app.UseFileServer(options);
/*app.UseDefaultFiles();*/

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
