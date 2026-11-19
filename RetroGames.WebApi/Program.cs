using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("String de conexão não configurada. Confira ConnectionStrings__Default no arquivo .env.");

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        opcoes.InvalidModelStateResponseFactory = contexto =>
            new BadRequestObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
            });
    });
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RetroGames API");
        options.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.Run();
