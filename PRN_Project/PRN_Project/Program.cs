using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LmsDbContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("StrCon")));

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

app.Use(async (context, next) =>
{
    //context.Session.SetInt32("AId", 1);
    context.Session.SetInt32("AId", 2); // lấy id
    context.Session.SetString("Role", "Student"); // giả lập role để đi đến trang cần thiết
    //context.Session.SetInt32("Role", 1);
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
