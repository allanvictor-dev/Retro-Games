using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<Usuario?> BuscarPorIdAsync(int id);
    Task<List<Usuario>> ListarPorIdsAsync(List<int> ids);
    Task<bool> ExisteAdministradorAsync();
    Task<int> InserirAsync(Usuario usuario);
}
