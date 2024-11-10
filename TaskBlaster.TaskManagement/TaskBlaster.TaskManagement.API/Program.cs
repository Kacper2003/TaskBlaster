using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.DAL;
using TaskBlaster.TaskManagement.DAL.Implementations;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.API.Services.Implementations;
using TaskBlaster.TaskManagement.Models.InputModels;
using TaskBlaster.TaskManagement.API.Utilities;
using TaskBlaster.TaskManagement.API.ExceptionHandlerExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Authentication configuration for JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Domain"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var claims = context.Principal?.Claims.Select(c => new { c.Type, c.Value });

                // Retrieve the custom claims from the token
                var email = context.Principal?.FindFirst("email_address")?.Value 
                            ?? throw new UnauthorizedAccessException("User email not found in token");
                var fullName = context.Principal?.FindFirst("full_name")?.Value ?? "Default Name";
                var profileImageUrl = context.Principal?.FindFirst("profile_image_url")?.Value;

                // Retrieve the user service from the DI container
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                // Create the user if they don't exist
                await userService.CreateUserIfNotExistsAsync(new UserInputModel
                {
                    EmailAddress = email,
                    FullName = fullName,
                    ProfileImageUrl = profileImageUrl
                });
            }
        };
    });

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPriorityRepository, PriorityRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register M2M authentication services
builder.Services.AddHttpClient("NotificationApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["NotificationApi:BaseUrl"] 
                                 ?? throw new InvalidOperationException("NotificationApi BaseUrl not configured."));
})
.AddHttpMessageHandler<M2MTokenHandler>();

builder.Services.AddTransient<IM2MAuthenticationService, M2MAuthenticationService>();
builder.Services.AddTransient<M2MTokenHandler>();

// Register DbContext
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TaskManagementDb")
    )
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
    services.Database.Migrate();
}

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

app.UseGlobalExceptionHandler(); // Exception handler

app.Run();
