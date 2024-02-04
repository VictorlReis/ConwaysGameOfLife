namespace ConwaysGameOfLife.Core.DTOs;

public record CellDto(int Id, int GameId, int Row, int Column, bool IsAlive);