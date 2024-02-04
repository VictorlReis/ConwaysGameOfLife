using ConwaysGameOfLife.Core.DTOs;

namespace ConwaysGameOfLife.Core.Services;

public interface IGameService
{ 
    Task<int> CreateNewGame(CreateNewGameDto createNewGameDto);
    Task<string> GetGameVisual(int gameId);
    Task<IEnumerable<GameDto>> GetAll();
    Task<GameDto?> Get(int gameId);
    Task AdvanceGenerations(int gameId, int generations = 1);
}