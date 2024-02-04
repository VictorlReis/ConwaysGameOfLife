using ConwaysGameOfLife.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwaysGameOfLife.Core.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext _context;

        public GameRepository(GameContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetGameById(int gameId)
        {
            return await _context.Games
                .Include(g => g.Cells)
                .FirstOrDefaultAsync(g => g.Id == gameId);
        }

        public async Task<IEnumerable<Game>> GetAll()
        {
            return await _context.Games.Include(g => g.Cells).ToListAsync();
        }

        public async Task<int> AddGame(Game game)
        {
             _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game.Id;
        }

        public async Task UpdateGame(Game? game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        public void DeleteGame(int gameId)
        {
            var game = _context.Games.FirstOrDefault(g => g.Id == gameId);
            if (game is null) return;
            _context.Games.Remove(game);
            _context.SaveChanges();
        }
    }
}