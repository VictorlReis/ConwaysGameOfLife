using ConwaysGameOfLife.Core.DTOs;
using ConwaysGameOfLife.Core.Entities;
using ConwaysGameOfLife.Core.Repositories;
using Newtonsoft.Json;

namespace ConwaysGameOfLife.Core.Services;
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository repository)
    {
        _gameRepository = repository;
    }
    public async Task<IEnumerable<GameDto>> GetAll()
    {
        var games = await _gameRepository.GetAll();
        return games.Select(x => x.ToDto());
    }

    public async Task<GameDto?> Get(int gameId)
    {
        var game = await _gameRepository.GetGameById(gameId);
        game.PrintBoard();
        return game?.ToDto();
    }
    public async Task<string> GetGameVisual(int gameId)
    {
        var game = await _gameRepository.GetGameById(gameId);

        if (game is null) return $"Game with ID {gameId} not found.";
        var visualRepresentation = new List<List<string>>();

        for (var i = 0; i < game.Rows; i++)
        {
            var rowRepresentation = new List<string>();

            for (var j = 0; j < game.Columns; j++)
            {
                var cell = game.Cells.FirstOrDefault(c => c.Row == i && c.Column == j);

                if (cell != null && cell.IsAlive) rowRepresentation.Add("X"); // Alive cell
                else rowRepresentation.Add("."); // Dead cell
            }
            visualRepresentation.Add(rowRepresentation);
        }

        var jsonResult = JsonConvert.SerializeObject(visualRepresentation);

        return jsonResult;
    }

    public async Task<int> CreateNewGame(CreateNewGameDto createNewGameDto)
    {
        var game = new Game();
        game.InitializeGame(createNewGameDto.BoardRows, createNewGameDto.BoardColumns);
        var gameId = await _gameRepository.AddGame(game);

        return gameId;
    }
    
    public async Task AdvanceGenerations(int gameId, int generations)
    {
        var game = await _gameRepository.GetGameById(gameId);

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
        await _gameRepository.UpdateGame(game);
    }
}
