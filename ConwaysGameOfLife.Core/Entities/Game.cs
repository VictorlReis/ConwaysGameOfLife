using ConwaysGameOfLife.Core.DTOs;

namespace ConwaysGameOfLife.Core.Entities;

public class Game
{
    public int Id { get; set; }
    public List<Cell> Cells { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int Generation { get; set; }
    public bool Finished { get; set; }

    
    
    public void InitializeGame(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Generation = 1;
        Finished = false;
        GenerateRandomCells();
    }
    
    public void PrintBoard()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                Cell cell = Cells.FirstOrDefault(c => c.Row == i && c.Column == j);

                var cellSymbol = cell?.IsAlive == true ? 'X' : '.';
                Console.Write(cellSymbol + " ");
            }
            Console.WriteLine();
        }
    }

    public GameDto? ToDto() => new(Id, Rows, Columns, Generation, Finished, Cells.Select(x => x.ToDto()));
    
    public int CountLiveNeighbors(int row, int column)
    {
        var liveNeighbors = 0;

        for (var i = row - 1; i <= row + 1; i++)
        {
            for (var j = column - 1; j <= column + 1; j++)
            {
                if (i == row && j == column) continue;

                var neighbor = Cells.FirstOrDefault(c => c.Row == i && c.Column == j);
                
                if (neighbor is { IsAlive: true }) liveNeighbors++;
            }
        }

        return liveNeighbors;
    }
    private void GenerateRandomCells()
    {
        var cells = new List<Cell>();
        var random = new Random();

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                var isAlive = random.Next(2) == 0;

                var cell = new Cell()
                {
                    Row = i,
                    Column = j,
                    IsAlive = isAlive,
                    Game = this
                };

                cells.Add(cell);
            }
        }

        Cells = cells;
    }
}
