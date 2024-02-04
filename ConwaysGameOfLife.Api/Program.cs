using ConwaysGameOfLife.Api.Extensions;
using ConwaysGameOfLife.Core.DTOs;
using ConwaysGameOfLife.Core.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/game/all", async (IGameService gameService) =>
{
       var allGames = await gameService.GetAll();
       return Results.Ok(allGames); 
});

app.MapGet("/game/{gameId}", async (IGameService gameService, int gameId) =>
{
    var game = await gameService.Get(gameId);
    return Results.Ok(game);
});

app.MapPost("/game/new", async (IGameService gameService, [FromBody] CreateNewGameDto createNewGameDto) =>
{
    var newGameId = await gameService.CreateNewGame(createNewGameDto);
    return Results.Ok(newGameId);
});

app.MapPost("/game/{gameId}/next", async (IGameService gameService, int gameId) =>
{
    await gameService.AdvanceGenerations(gameId);
    return Results.Ok();
});


app.MapPost("/game/{gameId}/next/{generations}", async (IGameService gameService, int gameId, int generations) =>
{
    await gameService.AdvanceGenerations(gameId, generations);
    return Results.Ok();
});

app.MapPost("/game/{gameId}/last", async (IGameService gameService, int gameId) =>
{
    return Results.Ok();
});

app.Run();