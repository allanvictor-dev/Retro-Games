using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new(MockBehavior.Strict);
    private readonly Mock<IPlataformaRepository> _plataformas = new(MockBehavior.Strict);
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new(MockBehavior.Strict);
    private readonly Mock<IGeneroRepository> _generos = new(MockBehavior.Strict);
    private readonly Mock<IJogoGeneroRepository> _vinculos = new(MockBehavior.Strict);
    private readonly JogoService _servico;

    public JogoServiceTests()
    {
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        repositorio.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        repositorio.SetupGet(r => r.CondicaoJogo).Returns(_condicoes.Object);
        repositorio.SetupGet(r => r.Genero).Returns(_generos.Object);
        repositorio.SetupGet(r => r.JogoGenero).Returns(_vinculos.Object);
        _servico = new JogoService(repositorio.Object, Mock.Of<IUsuarioAtualService>());
    }

    [Theory]
    [InlineData(99, 2, 3, 100)]
    [InlineData(1, 99, 3, 104)]
    [InlineData(1, 2, 99, 105)]
    public async Task InserirAsync_ReferenciaInexistente_RejeitaCadastro(
        int idPlataforma, int idCondicao, int idGenero, int codigoEsperado)
    {
        var jogo = new Jogo { Nome = "Sonic", IdPlataforma = idPlataforma, IdCondicaoJogo = idCondicao };
        _plataformas.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });
        _plataformas.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Plataforma?)null);
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync([new CondicaoJogo { Id = 2 }]);
        _generos.Setup(r => r.ListarAsync()).ReturnsAsync([new Genero { Id = 3 }, new Genero { Id = 4 }]);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(jogo, [4, idGenero]));

        Assert.Equal(codigoEsperado, erro.Erro.Codigo);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
        _vinculos.Verify(r => r.InserirAsync(It.IsAny<List<JogoGenero>>()), Times.Never);
    }

    [Fact]
    public async Task BuscarPorIdAsync_JogoInexistente_LancaErro103()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.BuscarPorIdAsync(99));

        Assert.Equal(103, erro.Erro.Codigo);
    }

    [Fact]
    public async Task DesativarAsync_JogoInexistente_LancaErro103SemDesativar()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.DesativarAsync(99));

        Assert.Equal(103, erro.Erro.Codigo);
        _jogos.Verify(r => r.DesativarAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ListarAsync_ComGenero_EnviaSomenteIdsDistintosDoGenero()
    {
        ConfigurarDetalhesVazios();
        _vinculos.Setup(r => r.ListarTodosAsync()).ReturnsAsync([
            new() { IdJogo = 12, IdGenero = 3 },
            new() { IdJogo = 15, IdGenero = 4 },
            new() { IdJogo = 12, IdGenero = 3 },
            new() { IdJogo = 18, IdGenero = 3 }
        ]);
        _jogos.Setup(r => r.ListarAsync(1, 5, null, null,
            It.Is<List<int>?>(ids => ids != null && ids.Order().SequenceEqual(new[] { 12, 18 })), false))
            .ReturnsAsync((new List<Jogo>(), 0)).Verifiable();

        await _servico.ListarAsync(idGenero: 3);

        _jogos.Verify();
    }

    [Fact]
    public async Task ListarAsync_SemGenero_EnviaIdsNulos()
    {
        ConfigurarDetalhesVazios();
        _jogos.Setup(r => r.ListarAsync(1, 5, null, null, null, false))
            .ReturnsAsync((new List<Jogo>(), 0)).Verifiable();

        await _servico.ListarAsync();

        _jogos.Verify();
        _vinculos.Verify(r => r.ListarTodosAsync(), Times.Never);
    }

    private void ConfigurarDetalhesVazios()
    {
        _plataformas.Setup(r => r.ListarAsync()).ReturnsAsync([]);
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync([]);
        _generos.Setup(r => r.ListarAsync()).ReturnsAsync([]);
        _vinculos.Setup(r => r.ListarPorJogosAsync(It.IsAny<List<int>>())).ReturnsAsync([]);
    }

    [Fact]
    public async Task ListarAsync_PaginaComDoisJogos_MontaDetalhesEPreservaTotal()
    {
        List<Jogo> pagina = [
            new() { Id = 11, IdPlataforma = 2, IdCondicaoJogo = 7 },
            new() { Id = 10, IdPlataforma = 1, IdCondicaoJogo = 8 }
        ];
        _jogos.Setup(r => r.ListarAsync(2, 2, null, null, null, false)).ReturnsAsync((pagina, 9));
        _plataformas.Setup(r => r.ListarAsync()).ReturnsAsync([
            new() { Id = 1, Nome = "Mega Drive" }, new() { Id = 2, Nome = "SNES" }
        ]);
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync([
            new() { Id = 8, Descricao = "Novo" }, new() { Id = 7, Descricao = "Usado" }
        ]);
        _generos.Setup(r => r.ListarAsync()).ReturnsAsync([
            new() { Id = 3, Nome = "RPG" }, new() { Id = 4, Nome = "Aventura" }
        ]);
        _vinculos.Setup(r => r.ListarPorJogosAsync(
            It.Is<List<int>>(ids => ids.Order().SequenceEqual(new[] { 10, 11 })))).ReturnsAsync([
            new() { IdJogo = 10, IdGenero = 4 }, new() { IdJogo = 11, IdGenero = 3 }
        ]);

        var (jogos, total) = await _servico.ListarAsync(pagina: 2, tamanhoPagina: 2);

        Assert.Equal(9, total);
        Assert.Collection(jogos,
            primeiro =>
            {
                Assert.Equal(11, primeiro.Jogo.Id);
                Assert.Equal("SNES", primeiro.NomePlataforma);
                Assert.Equal("Usado", primeiro.DescricaoCondicao);
                Assert.Equal("RPG", Assert.Single(primeiro.Generos).Nome);
            },
            segundo =>
            {
                Assert.Equal(10, segundo.Jogo.Id);
                Assert.Equal("Mega Drive", segundo.NomePlataforma);
                Assert.Equal("Novo", segundo.DescricaoCondicao);
                Assert.Equal("Aventura", Assert.Single(segundo.Generos).Nome);
            });
    }
}
