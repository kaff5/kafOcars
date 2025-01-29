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
builder.Services.AddSingleton(typeof(CoreLib.Db.IDbContextFactory<>), typeof(PostgresDbContextFactory<>));



// Database Contexts
//builder.Services.AddDbContext<ServiceRegistryDbContext>(options =>
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