using DotNetEnv;
using RetroGames.Repository;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Configuração da conexão com o MySQL
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("String de conexão não configurada.");

builder.Services.AddScoped(_ => new DatabaseConnection(connectionString));

// Configuração dos serviços da API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configuração da aplicação
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
