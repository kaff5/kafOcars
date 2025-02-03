using CoreLib.Db;
using CoreLib.Infrastucture;
using KafOCars.AuthorizationService.Controllers;
using KafOCars.AuthorizationService.DataAccess.Repositories;
using KafOCars.AuthorizationService.Services;
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

var databaseSettings = new DatabaseSettings
{
    MasterConnection = builder.Configuration.GetSection("ConnectionStrings:Master").Value ?? string.Empty,
    ReplicaConnections = builder.Configuration.GetSection("ConnectionStrings:ReplicaConnections").Get<string[]>() ?? Array.Empty<string>(),
    BalancerConnection = builder.Configuration.GetSection("ConnectionStrings:MasterAndReplica").Value ?? string.Empty,
};

// Явно регистрируем DatabaseSettings в DI-контейнере
builder.Services.AddSingleton(databaseSettings);

// Регистрация PostgresDbContextFactory
builder.Services.AddSingleton(typeof(CoreLib.Db.IDbContextFactory<AuthDbContext>), typeof(PostgresDbContextFactory<AuthDbContext>));



// Database Contexts
//builder.Services.AddDbContext<AuthDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("ServiceRegistry")));



//builder.Services.AddScoped<ShardResolver>();

//builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("Service"));


//оставим на позднее
// builder.Services.AddDbContext<AuthDbContext>((serviceProvider, options) =>
// {
//     var shardResolver = serviceProvider.GetRequiredService<ShardResolver>();
//     var connectionString = shardResolver.ResolveShardConnectionString();
//     options.UseNpgsql(connectionString);
// });

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Register gRPC services
    endpoints.MapGrpcService<AuthServiceController>();
    
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client.");
    });
});

app.Run();