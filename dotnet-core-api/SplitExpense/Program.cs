

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using SplitExpense.Data;
using SplitExpense.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure NLog as the logging provider
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the container.
builder.Services.RegisterService();

builder.Services.AddControllers();

var environment = builder.Environment.EnvironmentName;
builder.Configuration.RegisterConfiguration(environment);
  

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database context or other services if needed
// Add DbContext to services
builder.Services.AddDbContext<SplitExpenseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SplitExpenseDbContext>()
    .AddDefaultTokenProviders();

// Add authentication and external login providers
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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
 })
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});

// Register factory
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>(sp =>
{
    var options = sp.GetRequiredService<DbContextOptions<SplitExpenseDbContext>>();
    return new DbContextFactory(options);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<LoggingMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/GetEnvironment", (IWebHostEnvironment env) => $"Environment: {env.EnvironmentName}");

app.Run();
