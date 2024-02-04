using System.Net;
using ConwaysGameOfLife.Api.Extensions;
using ConwaysGameOfLife.Core.Requests;
using ConwaysGameOfLife.Core.Services;
using Microsoft.AspNetCore.Mvc;
using CreateNewGameRequest = ConwaysGameOfLife.Core.Requests.CreateNewGameRequest;

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
   var response = await gameService.GetAll();
    return response.StatusCode switch
    {
        404 => Results.NotFound(response.Message),
        500 => Results.Problem(response.Message, statusCode:500),
        _ => Results.Ok(response.Games)
    };
});

app.MapGet("/game/{gameId}", async (IGameService gameService, int gameId) =>
{
    var response = await gameService.Get(gameId);
    return response.StatusCode switch
    {
        404 => Results.NotFound(response.Message),
        500 => Results.Problem(response.Message, statusCode:500),
        _ => Results.Ok(response.Game)
    };
});

app.MapPost("/game/new", async (IGameService gameService, [FromBody] CreateNewGameRequest createNewGameRequest) =>
{
    var response = await gameService.CreateNewGame(createNewGameRequest);
    return response.StatusCode switch
    {
        500 => Results.Problem(response.Message, statusCode:500),
        _ => Results.Created($"/games/{response.GameId}", response.GameId)
    };
});

app.MapPost("/game/{gameId}/next", async (IGameService gameService, int gameId) =>
{
    var response = await gameService.AdvanceGenerations(new AdvanceGenerationsRequest(gameId));
    return response.StatusCode switch
    {
        400 => Results.BadRequest(response.Message),
        404 => Results.NotFound(response.Message),
        500 => Results.Problem(response.Message, statusCode:500),
        _ => Results.Ok(response.Game)
    };
});


app.MapPost("/game/{gameId}/next/{generations}", async (IGameService gameService, AdvanceGenerationsRequest request) =>
{
    var response = await gameService.AdvanceGenerations(request);
    return response.StatusCode switch
    {
        400 => Results.BadRequest(response.Message),
        404 => Results.NotFound(response.Message),
        500 => Results.Problem(response.Message, statusCode:500),
        _ => Results.Ok(response.Game)
    };
});

app.MapPost("/game/{gameId}/end", async (IGameService gameService, int gameId) =>
{
    var response = await gameService.EndGame(gameId);
    return response.StatusCode switch
    {
        400 => Results.BadRequest(response.Message),
        404 => Results.NotFound(response.Message),
        500 => Results.Problem(response.Message, statusCode:500),
        _ => Results.Ok(response.Message)
    };
});

app.Run();