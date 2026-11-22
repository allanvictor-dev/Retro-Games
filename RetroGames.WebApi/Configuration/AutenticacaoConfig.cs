using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AddAutenticacao(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                opcoes.Events.OnRedirectToLogin = contexto =>
                    ResponderErroAsync(contexto.Response, StatusCodes.Status401Unauthorized,
                        ResponseCodes.NAO_AUTENTICADO);

                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                    ResponderErroAsync(contexto.Response, StatusCodes.Status403Forbidden,
                        ResponseCodes.ACESSO_NEGADO);
            });

        return services;
    }

    private static Task ResponderErroAsync(HttpResponse resposta, int status,
        (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage
        });
    }
}
