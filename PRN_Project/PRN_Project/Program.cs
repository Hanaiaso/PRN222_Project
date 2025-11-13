using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Interfaces;
using PRN_Project.Repositories;
using PRN_Project.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LmsDbContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("StrCon")));

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");


}
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
app.UseStaticFiles(); //cho phép truy cập đến wwwroot
app.UseRouting(); //cho phép định tuyến các requests từ clinent
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
