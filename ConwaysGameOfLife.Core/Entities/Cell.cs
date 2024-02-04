using ConwaysGameOfLife.Core.DTOs;

namespace ConwaysGameOfLife.Core.Entities;
public class Cell
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsAlive { get; set; }
    
    public Game Game { get; set; }

    public CellDto ToDto() => new (Id, GameId, Row, Column, IsAlive);
}
