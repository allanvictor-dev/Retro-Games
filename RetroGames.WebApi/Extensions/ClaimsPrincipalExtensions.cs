using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil switch
        {
            PerfilIds.Administrador => Perfis.Administrador,
            PerfilIds.Cliente => Perfis.Cliente,
            _ => throw new InvalidOperationException("Perfil de usuário inválido.")
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, perfil)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme));
    }

    public static int? ObterId(this ClaimsPrincipal usuario)
    {
        return int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id : null;
    }
}
