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

    Task<List<Jogo>> ListarPorIdsAsync(List<int> ids);

    Task<int> InserirAsync(Jogo jogo);

    Task AlterarAsync(Jogo jogo);

    Task BaixarEstoqueAsync(int idJogo, int quantidade);

    Task DesativarAsync(int id);
}
