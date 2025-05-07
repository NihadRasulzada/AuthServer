using AuthServer.Core.Configuration;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<Client>(builder.Configuration.GetSection("CLients"));

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
