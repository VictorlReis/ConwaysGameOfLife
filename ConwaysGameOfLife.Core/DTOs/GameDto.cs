namespace ConwaysGameOfLife.Core.DTOs;

public record GameDto(int Id, int Rows, int Columns, int Generation, bool Finished, int CellsAlive);