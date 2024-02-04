using System.Reflection;
using ConwaysGameOfLife.Core;
using ConwaysGameOfLife.Core.Repositories;
using ConwaysGameOfLife.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace ConwaysGameOfLife.Api.Extensions;

public static class ApplicationServiceExtensions
{
    
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("gameSqlite");
        services.AddDbContext<GameContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        var gameConfigs = configuration.GetSection("GameConfigs");
        services.Configure<GameConfigs>(gameConfigs);
        
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGameRepository, GameRepository>();
    }
}