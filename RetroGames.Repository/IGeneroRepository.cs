using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarAsync();

    Task<Genero?> BuscarPorIdAsync(int id);

    Task<bool> ExisteComNomeAsync(
        string nome,
        int? idIgnorado = null
    );

    Task<bool> PossuiJogosAsync(int id);

    Task<int> InserirAsync(Genero genero);

    Task AlterarAsync(Genero genero);

    Task ExcluirAsync(int id);
}
