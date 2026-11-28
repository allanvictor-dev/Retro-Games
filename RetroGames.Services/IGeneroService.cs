using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IGeneroService
{
    Task<List<Genero>> ListarAsync();
    Task<Genero> BuscarPorIdAsync(int id);
    Task<Genero> InserirAsync(Genero genero);
    Task AlterarAsync(Genero genero);
    Task ExcluirAsync(int id);
}
