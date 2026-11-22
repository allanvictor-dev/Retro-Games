using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IUsuarioAtualService _usuarioAtual;

    public JogoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        int pagina = 1, int tamanhoPagina = 5, string? nome = null,
        int? idPlataforma = null, int? idGenero = null, bool incluirInativos = false)
    {
        List<int>? ids = null;

        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            ids = vinculos.Where(v => v.IdGenero == idGenero.Value)
                .Select(v => v.IdJogo).Distinct().ToList();
        }

        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            pagina, tamanhoPagina, nome, idPlataforma, ids,
            incluirInativos && _usuarioAtual.EhAdministrador);

        return (await MontarDetalhesAsync(jogos), total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await BuscarJogoAsync(id);
        if (!jogo.Ativo && !_usuarioAtual.EhAdministrador)
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });
        return detalhes[0];
    }

    public async Task<Jogo> InserirAsync(Jogo jogo, List<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await ValidarReferenciasAsync(jogo, idsGeneros);
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(CriarVinculos(jogo.Id, idsGeneros));
            transactionScope.Complete();
        }

        return jogo;
    }

    public async Task AlterarAsync(Jogo jogo, List<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            var existente = await BuscarJogoAsync(jogo.Id);
            jogo.DataCadastro = existente.DataCadastro;
            await ValidarReferenciasAsync(jogo, idsGeneros);

            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(CriarVinculos(jogo.Id, idsGeneros));
            transactionScope.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        await BuscarJogoAsync(id);
        await _repositorio.Jogo.DesativarAsync(id);
    }

    private async Task<Jogo> BuscarJogoAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null)
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);

        return jogo;
    }

    private async Task ValidarReferenciasAsync(Jogo jogo, List<int> idsGeneros)
    {
        if (await _repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) == null)
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
            throw new BusinessRuleException(BusinessErrors.CondicaoNaoEncontrada);

        var generos = await _repositorio.Genero.ListarAsync();
        if (idsGeneros.Any(id => !generos.Any(g => g.Id == id)))
            throw new BusinessRuleException(BusinessErrors.GeneroNaoEncontrado);
    }

    private static List<JogoGenero> CriarVinculos(int idJogo, List<int> idsGeneros)
    {
        return idsGeneros.Distinct().Select(id => new JogoGenero
        {
            IdJogo = idJogo,
            IdGenero = id
        }).ToList();
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(
            jogos.Select(j => j.Id).ToList());

        return jogos.Select(jogo => new JogoDetalheServiceModel
        {
            Jogo = jogo,
            NomePlataforma = plataformas.First(p => p.Id == jogo.IdPlataforma).Nome,
            DescricaoCondicao = condicoes.First(c => c.Id == jogo.IdCondicaoJogo).Descricao,
            Generos = (from vinculo in vinculos
                       join genero in generos on vinculo.IdGenero equals genero.Id
                       where vinculo.IdJogo == jogo.Id
                       select genero).ToList()
        }).ToList();
    }
}
