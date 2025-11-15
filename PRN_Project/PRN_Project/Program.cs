using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN_Project.Hubs;
using PRN_Project.Models;
using PRN_Project.Repositories;
using PRN_Project.Repositories.Implementations;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services;
using PRN_Project.Repositories.Implementations;
using PRN_Project.Services.Implementations;
using PRN_Project.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// ======= 1️⃣ Cấu hình MVC và DBContext =======
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LmsDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("StrCon"))
);

// ======= Đăng ký Session =======
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

// ======= Đăng ký SignalR =======
builder.Services.AddSignalR();




// ======= Đăng ký Dependency Injection cho Repository và Service =======

// Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IMockExamRepository, MockExamRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository >();
builder.Services.AddScoped<ILearningMaterialRepository, LearningMaterialRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITeacherClassroomService, TeacherClassroomService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();


// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMockExamService, MockExamService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<ILearningMaterialService, LearningMaterialService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IRankingRepository, RankingRepository>();
builder.Services.AddScoped<IRankingService, RankingService>();

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<ITeacherClassroomRepository, TeacherClassroomRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

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

// Thêm MapHub trước khi MapControllerRoute
app.MapHub<PRN_Project.Hubs.PrivateChatHub>("/privateChatHub");
app.MapHub<CommunityChatHub>("/communityChatHub");
app.MapHub<GroupChatHub>("/groupChatHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
