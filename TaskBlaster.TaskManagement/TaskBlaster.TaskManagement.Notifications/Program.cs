using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hangfire;
using Hangfire.PostgreSql;
using TaskBlaster.TaskManagement.Notifications.Services.Interfaces;
using TaskBlaster.TaskManagement.Notifications.Services.Implementations;
using TaskBlaster.TaskManagement.DAL; 
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.DAL.Implementations;
using TaskBlaster.TaskManagement.Notifications.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Domain"];
        options.Audience = builder.Configuration["Auth0:Audience"];
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services and repositories
builder.Services.AddSingleton<IMailService, MailjetService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register DbContext
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TaskManagementDb")));

// Configure Hangfire to use PostgreSQL storage
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireDb"))
);
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new AllowAllAuthorizationFilter()]
});

// Configure recurring job to send daily reports
RecurringJob.AddOrUpdate<INotificationService>(
    "SendDailyReports",
    service => service.SendDailyReports(),
    "*/30 * * * *"
);
app.Run();
