using ConwaysGameOfLife.Core.DTOs;
using ConwaysGameOfLife.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConwaysGameOfLife.Core.Services;
public class GameService : IGameService
{
    private readonly GameContext _context;

    public GameService(GameContext context)
    {
        _context = context;
    }
    public async Task<string> GetGameVisual(int gameId)
    {
        var game = await _context.Games
            .Include(g => g.Cells)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game != null)
        {
            var visualRepresentation = new List<List<string>>();

            for (var i = 0; i < game.Rows; i++)
            {
                var rowRepresentation = new List<string>();

                for (var j = 0; j < game.Columns; j++)
                {
                    var cell = game.Cells.FirstOrDefault(c => c.Row == i && c.Column == j);

                    if (cell != null && cell.IsAlive)
                    {
                        rowRepresentation.Add("X"); // Alive cell
                    }
                    else
                    {
                        rowRepresentation.Add("."); // Dead cell
                    }
                }

                visualRepresentation.Add(rowRepresentation);
            }

            var jsonResult = JsonConvert.SerializeObject(visualRepresentation);

            return jsonResult;
        }

        return $"Game with ID {gameId} not found.";
    }
    private void Get(int gameId)
    {
        var game = _context.Games
            .Include(g => g.Cells)
            .FirstOrDefault(g => g.Id == gameId);

        if (game != null)
        {
            Console.WriteLine($"Game ID: {game.Id}");
            Console.WriteLine($"Generation: {game.Generation}");
            Console.WriteLine("Current State:");

            foreach (var cell in game.Cells)
            {
                Console.WriteLine($"Row: {cell.Row}, Column: {cell.Column}, Alive: {cell.IsAlive}");
            }
        }
        else
        {
            Console.WriteLine($"Game with ID {gameId} not found.");
        }
    }
    public async Task<int> CreateNewGame(CreateNewGameDto createNewGameDto)
    {
        var game = new Game(createNewGameDto.BoardRows, createNewGameDto.BoardColumns);
        
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        Get(game.Id);

        return game.Id;
    }
}
