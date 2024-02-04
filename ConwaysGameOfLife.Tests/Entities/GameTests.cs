using ConwaysGameOfLife.Core.Entities;
using Moq;

namespace ConwaysGameOfLife.Tests.Entities;

public class GameTests
{
    [Theory]
    [InlineData(3, 3)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(8, 12)]
    [InlineData(15, 7)]
    public void InitializeGame_SetsPropertiesAndGeneratesCells(int rows, int columns)
    {
        var game = new Game();

        game.InitializeGame(rows, columns);

        Assert.Equal(rows, game.Rows);
        Assert.Equal(columns, game.Columns);
        Assert.Equal(1, game.Generation);
        Assert.False(game.Finished);
        Assert.True(game.Cells.Count > 0);
        Assert.Equal(game.Cells.Count, game.Rows * game.Columns);
    }
    
    [Theory]
    [InlineData(0, 0, 3)]
    [InlineData(1, 0, 5)]
    [InlineData(1, 1, 8)]
    [InlineData(2, 2, 3)]
    [InlineData(0, 2, 3)]
    [InlineData(2, 0, 3)]  
    [InlineData(0, 1, 5)] 
    [InlineData(2, 1, 5)]
    [InlineData(1, 2, 5)]
    [InlineData(0, 0, 0, false)]
    [InlineData(1, 0, 0, false)]
    [InlineData(2, 2, 0, false)]
    public void CountLiveNeighbors_ReturnsCorrectValue(int row, int column, int expectedLiveNeighbors, bool alive = true)
    {
        var game = new Game();
        game.InitializeGame(3, 3);

        game.Cells[0].IsAlive = alive;
        game.Cells[1].IsAlive = alive;
        game.Cells[2].IsAlive = alive;
        game.Cells[3].IsAlive = alive;
        game.Cells[4].IsAlive = alive;
        game.Cells[5].IsAlive = alive;
        game.Cells[6].IsAlive = alive;
        game.Cells[7].IsAlive = alive;
        game.Cells[8].IsAlive = alive;

        var liveNeighbors = game.CountLiveNeighbors(row, column);

        Assert.Equal(expectedLiveNeighbors, liveNeighbors);
    }
}

// 0,0 0,1 0,2
// 1,0 1,1 1,2
// 2,0 2,1 2,2

//0,0 0,1 0,2
//1,0     1,2
//2,0 2,1 2,2