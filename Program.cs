using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Services;
using Microsoft.AspNetCore.Identity;
using MyProjectIT15.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyProjectIT15.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDBContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
}
);

builder.Services.AddTransient<IEmailSender, EmailSender>();
//builder.Services.AddHttpClient<PayMongoService>();
builder.Services.AddSingleton<PayMongoService>();

builder.Services.Configure<PayMongoSettings>(
    builder.Configuration.GetSection("PayMongo"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the timeout duration
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); // Lockout duration
    options.Lockout.MaxFailedAccessAttempts = 3; // Maximum failed attempts
    options.Lockout.AllowedForNewUsers = true;   // Enable lockout for new users
} )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";  // Redirect to login page after timeout
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Total timeout duration
    options.SlidingExpiration = true;      // Reset expiration on each request
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession(); // Enable session management
app.UseSessionTimeout(); // Custom session timeout middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
