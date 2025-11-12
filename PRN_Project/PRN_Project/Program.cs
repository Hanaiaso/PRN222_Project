using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN_Project.Hubs;
using PRN_Project.Models;

var builder = WebApplication.CreateBuilder(args);

// ======= 1️⃣ Cấu hình MVC và DBContext =======
builder.Services.AddControllersWithViews();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<LmsDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("StrCon"))
);

builder.Services.AddSignalR();

// ======= 2️⃣ Cấu hình JWT Authentication =======
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    // Cho phép đọc token từ cookie nếu bạn lưu token trong cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
var app = builder.Build();

// ======= 3️⃣ Middleware pipeline =======
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
// Bắt buộc theo thứ tự này
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Thêm MapHub trước khi MapControllerRoute
app.MapHub<PRN_Project.Hubs.ChatHub>("/chatHub");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
