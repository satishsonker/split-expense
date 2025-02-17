using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SplitExpense.AutoMapperMapping;
using SplitExpense.Data;
using SplitExpense.Middleware;
using SplitExpense.ExceptionManagement.Exceptions;
using NLog.Web;
using SplitExpense.Models.ConfigModels;
using SplitExpense.FileManagement.Storage;
using SplitExpense.FileManagement.Service;
using SplitExpense.Middleware.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure NLog as the logging provider
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllers();

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
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<Mapping>());

// Add Swagger configuration
builder.Services.AddSwaggerConfiguration();

// Add authentication and external login providers
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
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(
             System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
 });
//.AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//})
//.AddFacebook(options =>
//{
//    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
//    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
//});

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
app.UseAuthorization();

app.MapControllers();
app.MapGet("/GetEnvironment", (IWebHostEnvironment env) => $"Environment: {env.EnvironmentName}");

app.Run();
