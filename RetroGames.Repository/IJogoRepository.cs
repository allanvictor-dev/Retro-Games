using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina = 1,
        int tamanhoPagina = 5,
        string? nome = null,
        int? idPlataforma = null,
        List<int>? ids = null,
        bool incluirInativos = false);

    Task<Jogo?> BuscarPorIdAsync(int id);

    Task<int> InserirAsync(Jogo jogo);

    Task AlterarAsync(Jogo jogo);

    Task DesativarAsync(int id);
}
