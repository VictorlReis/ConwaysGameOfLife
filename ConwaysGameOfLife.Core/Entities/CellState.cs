namespace ConwaysGameOfLife.Core.Entities;
public class CellState
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsAlive { get; set; }
}
