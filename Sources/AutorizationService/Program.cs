using CoreLib.Db;
using CoreLib.Infrastucture;
using KafOCars.AuthorizationService.Controllers;
using Microsoft.EntityFrameworkCore;
using SharedEntities.ServiceRegistry;
using KafOCars.DataAccess.Contexts;
using KafOCars.DataAccess.Repositories;
using KafOCars.DataAccess.Repositories.Interfaces;
using KafOCars.Services;
using KafOCars.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);



builder.Services.Configure<DatabaseSettings>(options =>
{
    options.MasterConnection = "Host=master.db;Database=mydb;Username=user;Password=pass";
    options.ReplicaConnections = new[]
    {
        "Host=replica1.db;Database=mydb;Username=user;Password=pass",
        "Host=replica2.db;Database=mydb;Username=user;Password=pass"
    };
});

// Регистрация PostgresDbContextFactory
// Используем для возможности read | write обращений к БД.
builder.Services.AddSingleton(typeof(CoreLib.Db.IDbContextFactory<>), typeof(PostgresDbContextFactory<>));


builder.Services.AddScoped<IUserReadRepository, UserReadRepository>();
builder.Services.AddScoped<IUserWriteRepository, UserWriteRepository>();
builder.Services.AddScoped<IRoleReadRepository, RoleReadRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddGrpc();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{

    endpoints.MapGrpcService<AuthServiceController>();
    
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client.");
    });
});

app.Run();