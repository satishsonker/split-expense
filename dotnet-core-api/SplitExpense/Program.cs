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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Add a custom header parameter for `userId`
    options.AddSecurityDefinition("userId", new OpenApiSecurityScheme
    {
        Name = "userId",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Custom header for UserId"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "userId"
                }
            },
            Array.Empty<string>()
        }
    });
});


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCustomExceptionHandler();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/GetEnvironment", (IWebHostEnvironment env) => $"Environment: {env.EnvironmentName}");

app.Run();
