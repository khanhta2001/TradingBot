using AspNetCore.Identity.Mongo;
using TradingBotAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddIdentityMongoDbProvider<UserAccount>(
    identityOptions =>
    {
        identityOptions.Password.RequireDigit = true;
        identityOptions.Password.RequiredLength = 8;
        identityOptions.Password.RequireNonAlphanumeric = true;
        identityOptions.Password.RequireUppercase = true;
        identityOptions.Password.RequireLowercase = true;
        identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        identityOptions.Lockout.MaxFailedAccessAttempts = 10;
    },
    mongoIdentityOptions =>
    {
        var configuration = builder.Configuration;
        mongoIdentityOptions.ConnectionString = configuration.GetSection("MongoDB:ConnectionUrl").Value;
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();