using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var resposta = new RetroGamesResponseApiModel<object>();

        if (exception is BusinessRuleException erroNegocio)
        {
            httpContext.Response.StatusCode = (int)erroNegocio.Erro.Status;
            resposta.ResponseCode = erroNegocio.Erro.Codigo;
            resposta.ErrorMessage = erroNegocio.Message;
        }
        else
        {
            _logger.LogError(exception, "Erro inesperado na API.");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            resposta.ResponseCode = ResponseCodes.ERRO;
            resposta.ErrorMessage = "Ocorreu um erro interno.";
        }

        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);
        return true;
    }
}
