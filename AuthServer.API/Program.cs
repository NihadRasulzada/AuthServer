using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));  
builder.Services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));

builder.Services.AddScoped<IUnitOfWork,  UnitOfWork>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"), contextOption =>
    {
        contextOption.MigrationsAssembly(nameof(AuthServer.Data));
    });
});

builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("CLients"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
