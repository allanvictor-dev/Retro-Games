using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task ListarAsync_RetornaCondicoesDoRepositorio()
    {
        var condicoes = new Mock<ICondicaoJogoRepository>();
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(r => r.CondicaoJogo).Returns(condicoes.Object);
        var servico = new CondicaoJogoService(repositorio.Object);
        List<CondicaoJogo> registros = [
            new() { Id = 7, Descricao = "Usado" },
            new() { Id = 2, Descricao = "Novo" }
        ];
        condicoes.Setup(r => r.ListarAsync()).ReturnsAsync(registros);

        var resultado = await servico.ListarAsync();

        Assert.Equal(registros.Select(x => (x.Id, x.Descricao)), resultado.Select(x => (x.Id, x.Descricao)));
        condicoes.Verify(r => r.ListarAsync(), Times.Once);
    }
}
