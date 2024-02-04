using ConwaysGameOfLife.Core;
using ConwaysGameOfLife.Core.Entities;
using ConwaysGameOfLife.Core.Repositories;
using ConwaysGameOfLife.Core.Requests;
using ConwaysGameOfLife.Core.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace ConwaysGameOfLife.Tests.Services;

public class GameServiceTests
{
    private readonly GameService _sut;
    private readonly Mock<IGameRepository> _gameRepository;
    private const int GameId = 1;
    public GameServiceTests()
    {
        _gameRepository = new Mock<IGameRepository>();
        var gameConfigs = new GameConfigs { MaxAttempts = 10 };
        var optionsMock = new Mock<IOptions<GameConfigs>>();
        optionsMock.Setup(x => x.Value).Returns(gameConfigs);
        _sut = new GameService(_gameRepository.Object, optionsMock.Object);
    } 
    
    [Fact]
    public async Task GetAll_ReturnsAllGames()
    {
        var game1 = new Game { Id = 1, Rows = 3, Columns = 3, Finished = false, Cells = new List<Cell>()};
        var game2 = new Game { Id = 2, Rows = 4, Columns = 4, Finished = false, Cells = new List<Cell>()};
        var games = new List<Game> { game1, game2 };

        _gameRepository.Setup(x => x.GetAll()).ReturnsAsync(games);

        var result = await _sut.GetAll();

        Assert.Null(result.Message);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Games);

        var gameDtos = result.Games.ToList();
        Assert.Equal(2, gameDtos.Count);

        Assert.Equal(game1.Id, gameDtos[0].Id);
        Assert.Equal(game1.Rows, gameDtos[0].Rows);
        Assert.Equal(game1.Columns, gameDtos[0].Columns);

        Assert.Equal(game2.Id, gameDtos[1].Id);
        Assert.Equal(game2.Rows, gameDtos[1].Rows);
        Assert.Equal(game2.Columns, gameDtos[1].Columns);
    }
    [Fact]
    public async Task GetAll_WhenEmpty_Returns404()
    {
        _gameRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<Game>());
        var result = await _sut.GetAll();

        Assert.Null(result.Games);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("No games found",result.Message);
    }
    [Fact]
    public async Task CreateNewGame_Success()
    {
        var request = new CreateNewGameRequest(3, 3);

        var result = await _sut.CreateNewGame(request);

        Assert.NotNull(result.GameId);
        _gameRepository.Verify(repo => repo.AddGame(It.IsAny<Game>()), Times.Once);
    }
    [Fact]
    public async Task CreateNewGame_WithInvalidRowsAndColumns_ReturnsBadRequest()
    {
        var request = new CreateNewGameRequest(-1, 3);

        var result = await _sut.CreateNewGame(request);

        Assert.Equal(404, result.StatusCode); 
        Assert.Null(result.GameId);
        _gameRepository.Verify(repo => repo.AddGame(It.IsAny<Game>()), Times.Never);
    }
    [Fact]
    public async Task AdvanceOneGeneration_Success()
    {
        var request = new AdvanceGenerationsRequest(GameId);

        var generations = 1;
        var game = new Game
        {
            Id = GameId,
            Rows = 3,
            Generation = generations,
            Columns = 3,
            Finished = false,
            Cells = new List<Cell>
            {
                new() { Row = 0, Column = 0, IsAlive = false },
                new() { Row = 0, Column = 1, IsAlive = true },
                new() { Row = 1, Column = 0, IsAlive = true },
                new() { Row = 1, Column = 1, IsAlive = true },
            }
        };
        _gameRepository.Setup(repo => repo.GetGameById(GameId)).ReturnsAsync(game);

        var result = await _sut.AdvanceGenerations(request);
        
        Assert.Equal(4, game.Cells.Count(x => x.IsAlive));
        Assert.Equal(generations + 1, game.Generation);
        Assert.Null(result.Message);
        Assert.Equal(200, result.StatusCode);
        _gameRepository.Verify(repo => repo.UpdateGame(game), Times.Once);
    }
    [Fact]
    public async Task AdvanceGenerations_GameNotFound()
    { 
        var request = new AdvanceGenerationsRequest(1, 5); 
        _gameRepository.Setup(repo => repo.GetGameById(request.GameId)).ReturnsAsync((Game)null);
        
        var result = await _sut.AdvanceGenerations(request);
        
        Assert.Null(result.Game); 
        Assert.Equal("Game not found", result.Message); 
        Assert.Equal(404, result.StatusCode); 
        _gameRepository.Verify(repo => repo.UpdateGame(It.IsAny<Game>()), Times.Never);
    }
    [Fact]
    public async Task AdvanceGenerations_GameAlreadyFinished()
    {
        var request = new AdvanceGenerationsRequest(GameId, 5); 
        var game = new Game 
        { 
            Id = GameId, 
            Finished = true
        }; 
        _gameRepository.Setup(repo => repo.GetGameById(GameId)).ReturnsAsync(game);
        var result = await _sut.AdvanceGenerations(request);
          
        Assert.Null(result.Game);
        Assert.Equal("This game has already been finished", result.Message);
        Assert.Equal(400, result.StatusCode);
        _gameRepository.Verify(repo => repo.UpdateGame(It.IsAny<Game>()), Times.Never); 
    }
    [Fact]
    public async Task AdvanceGenerations_Exception() 
    { 
        var request = new AdvanceGenerationsRequest(1, 5); 
        _gameRepository.Setup(repo => repo.GetGameById(request.GameId)).ThrowsAsync(new Exception("Generic ex error"));
        
        var result = await _sut.AdvanceGenerations(request);
        
        Assert.Null(result.Game); 
        Assert.Equal("An error occurred while advancing to the next generation", result.Message); 
        Assert.Equal(500, result.StatusCode); 
        _gameRepository.Verify(repo => repo.UpdateGame(It.IsAny<Game>()), Times.Never); // Ensure UpdateGame is not called
    }
    [Fact]
    public async Task EndGame_SuccessfullyConcluded()
    {
        var game = new Game { Id = GameId, Finished = false, Cells = new List<Cell> { new() { IsAlive = false } } };
        _gameRepository.Setup(repo => repo.GetGameById(GameId)).ReturnsAsync(game);
        
        var result = await _sut.EndGame(GameId);

        Assert.Equal($"Game with ID {GameId} concluded successfully.", result.Message);
        Assert.Equal(200, result.StatusCode);
        Assert.True(game.Finished);
        _gameRepository.Verify(repo => repo.UpdateGame(game), Times.Once);
    }
    [Fact]
    public async Task EndGame_AlreadyFinished()
    {
        var game = new Game { Id = GameId, Finished = true, Cells = new List<Cell> { new() { IsAlive = false } } };
        _gameRepository.Setup(repo => repo.GetGameById(GameId)).ReturnsAsync(game);
        
        var result = await _sut.EndGame(GameId);
        
        Assert.Equal("This game has already been finished", result.Message);
        Assert.Equal(400, result.StatusCode);
        _gameRepository.Verify(repo => repo.UpdateGame(It.IsAny<Game>()), Times.Never);
    }
    [Fact]
    public async Task EndGame_WithStableBlockPattern_ShouldNotEnd()
    {
        var game = new Game
        {
            Id = GameId,
            Rows = 3,
            Columns = 3,
            Finished = false,
            Cells = new List<Cell>
            {
                new() { Row = 0, Column = 0, IsAlive = true },
                new() { Row = 0, Column = 1, IsAlive = true },
                new() { Row = 1, Column = 0, IsAlive = true },
                new() { Row = 1, Column = 1, IsAlive = true },
            }
        };

        _gameRepository.Setup(repo => repo.GetGameById(GameId)).ReturnsAsync(game);

        var result = await _sut.EndGame(GameId);

        Assert.Equal($"Game with ID {GameId} did not reach a conclusion after 10 attempts.", result.Message);
        Assert.Equal(400, result.StatusCode);
        Assert.True(game.Finished);
        _gameRepository.Verify(repo => repo.UpdateGame(game), Times.Once);
    }
}