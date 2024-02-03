using ConwaysGameOfLife.Core;
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

        services.AddScoped<IGameService, GameService>();
    }
}