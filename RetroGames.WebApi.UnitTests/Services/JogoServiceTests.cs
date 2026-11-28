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
}
