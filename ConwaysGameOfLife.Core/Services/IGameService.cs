using ConwaysGameOfLife.Core.DTOs;
using ConwaysGameOfLife.Core.Requests;
using ConwaysGameOfLife.Core.Responses;

namespace ConwaysGameOfLife.Core.Services;

public interface IGameService
{ 
    Task<CreateNewGameResponse> CreateNewGame(CreateNewGameRequest request);
    Task<AllGamesResponse> GetAll();
    Task<GameResponse> Get(int gameId);
    Task<GameResponse> AdvanceGenerations(AdvanceGenerationsRequest request);
    Task<EndGameResponse> EndGame(int gameId);
}