namespace ConwaysGameOfLife.Core.Responses;

public record CreateNewGameResponse(int? GameId = null, int? StatusCode = null, string? Message = null);