using CoreLib.Db;
using KafOCars.AuthorizationService.Services;
using KafOCars.RegisterService.DataAccess.Repositories;
using KafOCars.RegisterService.DataAccess.Repositories.Interfaces;
using KafOCars.RegisterService.Services.Interfaces;
using Microsoft.OpenApi.Models;

namespace RegisterService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Настройка подключения к базе данных
            builder.Services.Configure<DatabaseSettings>(options =>
            {
                options.MasterConnection = "Host=master.db;Database=mydb;Username=user;Password=pass";
                options.ReplicaConnections = new[]
                {
                    "Host=replica1.db;Database=mydb;Username=user;Password=pass",
                    "Host=replica2.db;Database=mydb;Username=user;Password=pass"
                };
            });

            // Регистрация фабрики контекста БД для операций чтения/записи
            builder.Services.AddSingleton(typeof(IDbContextFactory<>), typeof(PostgresDbContextFactory<>));

            // Регистрация репозиториев и сервисов
            builder.Services.AddScoped<IUserReadRepository, UserReadRepository>();
            builder.Services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            builder.Services.AddScoped<IRoleReadRepository, RoleReadRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IRegisterService, KafOCars.RegisterService.Services.RegisterService>();

            // Добавление авторизации
            builder.Services.AddAuthorization();

            // Подключение Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Register Service API",
                    Version = "v1"
                });
            });

            var app = builder.Build();

            // Настройка middleware для Swagger в режиме разработки
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Register Service API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Здесь можно добавить свои эндпоинты или контроллеры

            app.Run();
        }
    }
}
