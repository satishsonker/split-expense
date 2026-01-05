using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using SplitExpense.AutoMapperMapping;
using SplitExpense.Data;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.FileManagement.Service;
using SplitExpense.FileManagement.Storage;
using SplitExpense.Middleware;
using SplitExpense.Middleware.Extensions;
using SplitExpense.Models.ConfigModels;
using SplitExpense.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure NLog as the logging provider
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
builder.Services
    .RegisterService()
    .AddDbContext<SplitExpenseDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))
    .RegisterBackgroundService();

var environment = builder.Environment.EnvironmentName;
builder.Configuration.RegisterConfiguration(environment);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.RegisterValidator();

// Add AutoMapper AddAutoMapper(cfg => cfg.AddProfile<SplitExpense.AutoMapperMapping.Mapping>());
builder.Services.AddAutoMapper(typeof(Mapping));

// Add Swagger configuration
builder.Services.AddSwaggerConfiguration();

// Add JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = builder.Configuration.GetValue<bool>("Jwt:EnableAuthentication"),
            ValidateAudience = builder.Configuration.GetValue<bool>("Jwt:EnableAuthentication"),
            ValidateLifetime = builder.Configuration.GetValue<bool>("Jwt:EnableAuthentication"),
            ValidateIssuerSigningKey = builder.Configuration.GetValue<bool>("Jwt:EnableAuthentication"),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? 
                    throw new ArgumentNullException("Jwt:Key")))
        };
    });

// In your service registration
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUpload"));

// Register the appropriate storage provider based on configuration
var storageType = builder.Configuration.GetValue<StorageType>("FileUpload:StorageSettings:StorageType");
switch (storageType)
{
    case StorageType.Local:
        builder.Services.AddScoped<IStorageProvider, LocalStorageProvider>();
        break;
    case StorageType.AzureBlob:
        builder.Services.AddScoped<IStorageProvider, AzureBlobStorageProvider>();
        break;
    case StorageType.AwsS3:
        builder.Services.AddScoped<IStorageProvider, AwsS3StorageProvider>();
        break;
    case StorageType.Api:
        builder.Services.AddScoped<IStorageProvider, ApiStorageProvider>();
        break;
}

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

builder.Services.AddAuthorization(options =>
{
    if (builder.Configuration.GetValue<bool>("Jwt:EnableAuthentication"))
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    }
    else
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAssertion(_ => true)
            .Build();
        
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAssertion(_ => true)
            .Build();
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

// Add static file middleware before routing
app.UseStaticFileConfiguration(builder.Configuration);

app.UseCors(option =>
{
    option.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
});
app.UseCustomExceptionHandler();

// Always add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/GetEnvironment", (IWebHostEnvironment env) => $"Environment: {env.EnvironmentName}");

app.Run();
