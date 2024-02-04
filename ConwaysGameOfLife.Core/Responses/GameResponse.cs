using ConwaysGameOfLife.Core.DTOs;
namespace ConwaysGameOfLife.Core.Responses;
public record GameResponse(string? Message = null, int StatusCode = 200, GameDto? Game = null);