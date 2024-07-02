using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Contexts;
using AuthServer.Data.Repositories;
using AuthServer.Service;
using AuthServer.Service.Services;
using AuthServer.Service.Servioces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.Configurations;
using SharedLibrary.Extensions;
using System.Collections.Generic;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region ConfigureServices

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("AuthServer.Data");
    });
});

builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));

var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
builder.Services.AddCustomTokenAuth(tokenOptions);

builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.UseCustomValidationResponse();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Configure

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCustomException();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
