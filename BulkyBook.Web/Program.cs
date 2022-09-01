
using BulkyBook.Common.Interfaces;
using BulkyBook.EntityFrameWorkDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using models = BulkyBook.Common.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using BulkyBook.Web.Utility;
using BulkyBook.Common.Common;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<BulkyBookDbContext>(options =>
options.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<FacebookSettings>(builder.Configuration.GetSection("Facebook"));
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["Facebook:AppKey"];
    options.AppSecret = builder.Configuration["Facebook:AppSecret"];
});

builder.Services.AddIdentity<models.Customer, IdentityRole>(options =>
 {
     options.SignIn.RequireConfirmedAccount = false;
     options.Password.RequireDigit = false;
     options.Password.RequiredLength = 6;
     options.Password.RequireNonAlphanumeric = false;
     options.Password.RequireUppercase = false;
     options.Password.RequireLowercase = false;
 }).AddDefaultTokenProviders()
 .AddEntityFrameworkStores<BulkyBookDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<IEmailSender, BulkyBookEmailSender>();
builder.Services.AddScoped<IUnitOfWork, BulkyBookUnitOfWork>();
builder.Services.AddScoped<IDbInitializer, BulkDbInitializer>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Prevents the website from getting loaded in iFrame
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

await SeedDatabase();
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var bulkExtensions = scope.ServiceProvider.GetService<IDbInitializer>();
        await bulkExtensions?.Initialise();
    }
}
