using DoAnWebApp.Data;
using DoAnWebApp.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DoAnWebAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DoAnWebAppContext") ?? throw new InvalidOperationException("Connection string 'DoAnWebAppContext' not found.")));

// Add services to the container.
builder.Services.AddScoped<DongHoService>();
builder.Services.AddScoped<NguoiDungService>();
builder.Services.AddScoped<MuaNgayService>();
builder.Services.AddScoped<ThanhToanServise>();

builder.Services.AddControllersWithViews();

// Bật Session
builder.Services.AddDistributedMemoryCache(); // lưu session trong bộ nhớ
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian tồn tại session
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

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DongHoes}/{action=Index}/{id?}");

app.Run();
