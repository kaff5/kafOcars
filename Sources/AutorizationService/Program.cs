using CoreLib.Db;

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


builder.Services.AddGrpc();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{

    //endpoints.MapGrpcService<AuthServiceController>();
    
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client.");
    });
});

app.Run();