namespace ConwaysGameOfLife.Core.Requests;

public record AdvanceGenerationsRequest(int GameId, int Generations =1);