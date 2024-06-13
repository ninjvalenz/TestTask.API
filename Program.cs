using Microsoft.Data.SqlClient;
using System.Data;
using TestTask.API.Authorization;
using TestTask.API.Helper;
using TestTask.API.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(configuration);
builder.Services.AddScoped<IDbConnection>(provider =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(x => x
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
