using System.Net;

namespace RetroGames.Services;

public record BusinessError(int Codigo, string Mensagem, HttpStatusCode Status);