using ConwaysGameOfLife.Core.DTOs;
namespace ConwaysGameOfLife.Core.Responses;
public record AllGamesResponse(string? Message = null, int StatusCode = 200, IEnumerable<GameDto?>? Games = null);