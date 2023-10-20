using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pink_Panthers_Project.Data;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Pink_Panthers_ProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Azure") ?? throw new InvalidOperationException("Connection string 'Pink_Panthers_ProjectContext' not found."), 
        pOptions => pOptions.EnableRetryOnFailure()));

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<String>();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
