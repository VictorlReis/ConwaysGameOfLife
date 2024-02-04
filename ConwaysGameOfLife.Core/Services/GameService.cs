using ConwaysGameOfLife.Core.Entities;
using ConwaysGameOfLife.Core.Repositories;
using ConwaysGameOfLife.Core.Requests;
using ConwaysGameOfLife.Core.Responses;
using Microsoft.Extensions.Options;

namespace ConwaysGameOfLife.Core.Services;
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly int _maxAttempts;
    public GameService(IGameRepository repository, IOptions<GameConfigs> config)
    {
        _gameRepository = repository;
        _maxAttempts = config.Value.MaxAttempts;
    }
    public async Task<AllGamesResponse> GetAll()
    {
        try
        {
            var games = await _gameRepository.GetAll();
            var gamesList = games.ToList();
            return gamesList.Count == 0 ? new AllGamesResponse("No games found", 404) : 
                new AllGamesResponse(Games: gamesList.Select(x => x.ToDto()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AllGamesResponse("An error occurred while getting the games", 500);
        }
    }
    public async Task<GameResponse> Get(int gameId)
    {
        var game = await _gameRepository.GetGameById(gameId);
        return game is null ? new GameResponse("Game not found", 404) : new GameResponse(Game: game.ToDto());
    }
    public async Task<CreateNewGameResponse> CreateNewGame(CreateNewGameRequest request)
    {
        if (request.BoardColumns <= 0 || request.BoardRows <= 0)
            return new CreateNewGameResponse(StatusCode: 404, Message: "Invalid Columns or Rows");
        try
        {
            var game = new Game();
            game.InitializeGame(request.BoardRows, request.BoardColumns);
            var gameId = await _gameRepository.AddGame(game);
            return new CreateNewGameResponse(gameId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new CreateNewGameResponse(StatusCode: 500, Message: "An error occurred while creating the game.");
        }
    }
    public async Task<EndGameResponse> EndGame(int gameId)
    {
          try
          {
              var game = await _gameRepository.GetGameById(gameId);
              if (game is null)
                  return new EndGameResponse($"Game with ID {gameId} not found.", 404);

              if (game.Finished)
                  return new EndGameResponse("This game has already been finished", 400);

              var currentAttempts = 0;

              while (currentAttempts < _maxAttempts)
              {
                  var cellsAlive = game.Cells.Count(x => x.IsAlive);

                  if (cellsAlive == 0)
                  {
                      game.Finished = true;
                      await _gameRepository.UpdateGame(game);
                      return new EndGameResponse($"Game with ID {gameId} concluded successfully.", 200);
                  }
                  
                  ProcessGeneration(game, (int)Math.Pow(2, currentAttempts));
                  currentAttempts++;
              }

              game.Finished = true;
              await _gameRepository.UpdateGame(game);
              return new EndGameResponse($"Game with ID {gameId} did not reach a conclusion after {_maxAttempts} attempts.", 400);
          }
          catch (Exception e)
          {
              Console.WriteLine(e);
              return new EndGameResponse( "An error occurred while processing the game.", 500);
          }
    }
    public async Task<GameResponse> AdvanceGenerations(AdvanceGenerationsRequest request)
    {
        try
        {
            var game = await _gameRepository.GetGameById(request.GameId);
            if (game is null)
                return new GameResponse( "Game not found", 404);
            if(game.Finished)
                  return new GameResponse("This game has already been finished", 400);
        
            ProcessGeneration(game, request.Generations);
            await _gameRepository.UpdateGame(game);
            return new GameResponse(Game: game.ToDto());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new GameResponse(Message:"An error occurred while advancing to the next generation", StatusCode:500);
        }
    }
    private static void ProcessGeneration(Game game, int generations)
    {
        while (generations > 0)
        {
            if (game is not { Finished: false }) continue;
            var newGenerationCells = new List<Cell>();

            foreach (var cell in game.Cells)
            {
                var liveNeighbors = game.CountLiveNeighbors(cell.Row, cell.Column);

                if (cell.IsAlive && liveNeighbors is < 2 or > 3)
                {
                    // Any live cell with fewer than two live neighbors dies, as if by underpopulation.
                    // Any live cell with more than three live neighbors dies, as if by overpopulation.
                    cell.IsAlive = false;
                }
                else if (!cell.IsAlive && liveNeighbors == 3)
                {
                    // Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
                    cell.IsAlive = true;
                }

                newGenerationCells.Add(new Cell
                {
                    Row = cell.Row,
                    Column = cell.Column,
                    IsAlive = cell.IsAlive,
                    Game = game
                });
            }

            game.Cells = newGenerationCells;
            game.Generation++;
            generations--;
        }
    }
}
