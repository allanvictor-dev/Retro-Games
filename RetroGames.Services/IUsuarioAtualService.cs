namespace RetroGames.Services;

public interface IUsuarioAtualService
{
    bool EstaLogado { get; }
    int? IdUsuario { get; }
    bool EhAdministrador { get; }
}
