using ConwaysGameOfLife.Core.DTOs;

namespace ConwaysGameOfLife.Core.Services;

public interface IGameService
{ 
    Task<int> CreateNewGame(CreateNewGameDto createNewGameDto);
    Task<string> GetGameVisual(int gameId);
}