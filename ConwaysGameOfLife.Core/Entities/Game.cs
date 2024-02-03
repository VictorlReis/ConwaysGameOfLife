namespace ConwaysGameOfLife.Core.Entities;

public class Game
{
    public int Id { get; set; }
    public List<CellState> CurrentState { get; set; }
    public int Generation { get; set; }
    public bool Finished { get; set; }
}
