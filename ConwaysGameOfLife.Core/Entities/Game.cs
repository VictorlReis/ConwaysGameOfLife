namespace ConwaysGameOfLife.Core.Entities;

public class Game
{
    public int Id { get; set; }
    public List<Cell> Cells { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int Generation { get; set; }
    public bool Finished { get; set; }

    public Game(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Generation = 1;
        Finished = false;
        GenerateRandomCells();
    }
    public void GenerateRandomCells()
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
