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

app.MapGet("/game/all", async (IGameService GameService) =>
{
    return Results.Ok();
});

app.MapGet("/game/{gameId}/current", async (IGameService GameService, int gameId) =>
{
    return Results.Ok();
});

app.MapPost("/game/new", async (IGameService GameService, [FromBody] CreateNewGameDto createNewGameDto) =>
{
    return Results.Ok();
});

app.MapPost("/game/{gameId}/next", async (IGameService GameService, int gameId) =>
{
    return Results.Ok();
});

app.MapPost("/game/{gameId}/last", async (IGameService GameService, int gameId) =>
{
    return Results.Ok();
});

app.Run();