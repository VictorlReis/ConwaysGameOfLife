using ConwaysGameOfLife.Core.DTOs;
using ConwaysGameOfLife.Core.Entities;

namespace ConwaysGameOfLife.Core.Services;

public interface IGameService
{ 
    Task<int> CreateNewGame(CreateNewGameDto createNewGameDto);
    Task<string> GetGameVisual(int gameId);
    Task<IEnumerable<GameDto>> GetAll();
    Task<GameDto?> Get(int gameId);
}