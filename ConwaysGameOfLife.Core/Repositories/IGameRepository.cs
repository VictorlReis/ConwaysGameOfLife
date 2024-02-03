using ConwaysGameOfLife.Core.Entities;

namespace ConwaysGameOfLife.Core.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAll();
    Task<Game?> GetGameById(int gameId);
    Task<int> AddGame(Game game);
    Task UpdateGame(Game game);
}