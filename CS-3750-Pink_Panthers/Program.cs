using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pink_Panthers_Project.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Pink_Panthers_ProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Titan_Server") ?? throw new InvalidOperationException("Connection string 'Pink_Panthers_ProjectContext' not found."), 
        pOptions => pOptions.EnableRetryOnFailure()));

// Add services to the container.
builder.Services.AddControllersWithViews();

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
