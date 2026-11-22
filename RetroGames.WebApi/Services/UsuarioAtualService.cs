using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _contexto;

    public UsuarioAtualService(IHttpContextAccessor contexto)
    {
        _contexto = contexto;
    }

    public bool EstaLogado => _contexto.HttpContext?.User.Identity?.IsAuthenticated == true;
    public int? IdUsuario => EstaLogado ? _contexto.HttpContext?.User.ObterId() : null;
    public bool EhAdministrador => EstaLogado &&
        _contexto.HttpContext?.User.IsInRole(Perfis.Administrador) == true;
}
