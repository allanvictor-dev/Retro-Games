using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioMock;
    private readonly Mock<IGeneroRepository> _generoRepoMock;
    private readonly GeneroService _service;

    public GeneroServiceTests()
    {
        _repositorioMock = new Mock<IRepositorySet>();
        _generoRepoMock = new Mock<IGeneroRepository>();
        _repositorioMock.Setup(r => r.Genero).Returns(_generoRepoMock.Object);
        _service = new GeneroService(_repositorioMock.Object);
    }

    [Fact]
    public async Task BuscarPorIdAsync_GeneroInexistente_LancaGeneroNaoEncontrado()
    {
        // Arrange
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(99))
            .ReturnsAsync((Genero?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.BuscarPorIdAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNaoEncontrado, excecao.Erro);
        _generoRepoMock.Verify(r => r.BuscarPorIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_GeneroExistente_RetornaGenero()
    {
        // Arrange
        var esperado = new Genero { Id = 1, Nome = "RPG" };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(esperado);

        // Act
        var resultado = await _service.BuscarPorIdAsync(1);

        // Assert
        Assert.Equal(esperado.Id, resultado.Id);
        Assert.Equal(esperado.Nome, resultado.Nome);
        _generoRepoMock.Verify(r => r.BuscarPorIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task InserirAsync_DadosValidos_InsereERetornaComId()
    {
        // Arrange
        var novoGenero = new Genero { Nome = "  Metroidvania  " };
        _generoRepoMock
            .Setup(r => r.ExisteComNomeAsync("Metroidvania", null))
            .ReturnsAsync(false);
        _generoRepoMock
            .Setup(r => r.InserirAsync(It.IsAny<Genero>()))
            .ReturnsAsync(10);

        // Act
        var resultado = await _service.InserirAsync(novoGenero);

        // Assert
        Assert.Equal(10, resultado.Id);
        Assert.Equal("Metroidvania", resultado.Nome);
        _generoRepoMock.Verify(r => r.ExisteComNomeAsync("Metroidvania", null), Times.Once);
        _generoRepoMock.Verify(
            r => r.InserirAsync(It.Is<Genero>(g => g.Nome == "Metroidvania")),
            Times.Once);
    }

    [Fact]
    public async Task InserirAsync_NomeDuplicado_LancaGeneroNomeDuplicado()
    {
        // Arrange
        var novoGenero = new Genero { Nome = "Ação" };
        _generoRepoMock
            .Setup(r => r.ExisteComNomeAsync("Ação", null))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.InserirAsync(novoGenero));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNomeDuplicado, excecao.Erro);
        _generoRepoMock.Verify(r => r.ExisteComNomeAsync("Ação", null), Times.Once);
        _generoRepoMock.Verify(r => r.InserirAsync(It.IsAny<Genero>()), Times.Never);
    }

    [Fact]
    public async Task InserirAsync_RegistroDuplicadoNoBanco_LancaGeneroNomeDuplicado()
    {
        // Arrange
        var novoGenero = new Genero { Nome = "Ação" };
        _generoRepoMock
            .Setup(r => r.ExisteComNomeAsync("Ação", null))
            .ReturnsAsync(false);
        _generoRepoMock
            .Setup(r => r.InserirAsync(It.IsAny<Genero>()))
            .ThrowsAsync(new RegistroDuplicadoException("Entrada duplicada"));

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.InserirAsync(novoGenero));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNomeDuplicado, excecao.Erro);
        _generoRepoMock.Verify(r => r.InserirAsync(It.IsAny<Genero>()), Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_MantendoProprioNome_AlteraComSucesso()
    {
        // Arrange
        var existente = new Genero { Id = 3, Nome = "Plataforma" };
        var atualizado = new Genero { Id = 3, Nome = "  Plataforma  " };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(3))
            .ReturnsAsync(existente);
        _generoRepoMock
            .Setup(r => r.ExisteComNomeAsync("Plataforma", 3))
            .ReturnsAsync(false);

        // Act
        await _service.AlterarAsync(atualizado);

        // Assert
        Assert.Equal("Plataforma", atualizado.Nome);
        _generoRepoMock.Verify(r => r.BuscarPorIdAsync(3), Times.Once);
        _generoRepoMock.Verify(r => r.ExisteComNomeAsync("Plataforma", 3), Times.Once);
        _generoRepoMock.Verify(
            r => r.AlterarAsync(It.Is<Genero>(g => g.Id == 3 && g.Nome == "Plataforma")),
            Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_NomeDuplicado_LancaGeneroNomeDuplicado()
    {
        // Arrange
        var existente = new Genero { Id = 3, Nome = "Plataforma" };
        var atualizado = new Genero { Id = 3, Nome = "RPG" };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(3))
            .ReturnsAsync(existente);
        _generoRepoMock
            .Setup(r => r.ExisteComNomeAsync("RPG", 3))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.AlterarAsync(atualizado));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNomeDuplicado, excecao.Erro);
        _generoRepoMock.Verify(r => r.ExisteComNomeAsync("RPG", 3), Times.Once);
        _generoRepoMock.Verify(r => r.AlterarAsync(It.IsAny<Genero>()), Times.Never);
    }

    [Fact]
    public async Task AlterarAsync_GeneroInexistente_LancaGeneroNaoEncontrado()
    {
        // Arrange
        var atualizado = new Genero { Id = 99, Nome = "RPG" };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(99))
            .ReturnsAsync((Genero?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.AlterarAsync(atualizado));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNaoEncontrado, excecao.Erro);
        _generoRepoMock.Verify(r => r.AlterarAsync(It.IsAny<Genero>()), Times.Never);
    }

    [Fact]
    public async Task AlterarAsync_RegistroDuplicadoNoBanco_LancaGeneroNomeDuplicado()
    {
        // Arrange
        var existente = new Genero { Id = 3, Nome = "Plataforma" };
        var atualizado = new Genero { Id = 3, Nome = "RPG" };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(3))
            .ReturnsAsync(existente);
        _generoRepoMock
            .Setup(r => r.ExisteComNomeAsync("RPG", 3))
            .ReturnsAsync(false);
        _generoRepoMock
            .Setup(r => r.AlterarAsync(It.IsAny<Genero>()))
            .ThrowsAsync(new RegistroDuplicadoException("Entrada duplicada"));

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.AlterarAsync(atualizado));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNomeDuplicado, excecao.Erro);
        _generoRepoMock.Verify(r => r.AlterarAsync(It.IsAny<Genero>()), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_GeneroComJogosAssociados_LancaGeneroPossuiJogos()
    {
        // Arrange
        var existente = new Genero { Id = 2, Nome = "Aventura" };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(2))
            .ReturnsAsync(existente);
        _generoRepoMock
            .Setup(r => r.PossuiJogosAsync(2))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.ExcluirAsync(2));

        // Assert
        Assert.Equal(BusinessErrors.GeneroPossuiJogos, excecao.Erro);
        _generoRepoMock.Verify(r => r.PossuiJogosAsync(2), Times.Once);
        _generoRepoMock.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_GeneroSemJogos_ExcluiComSucesso()
    {
        // Arrange
        var existente = new Genero { Id = 2, Nome = "Aventura" };
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(2))
            .ReturnsAsync(existente);
        _generoRepoMock
            .Setup(r => r.PossuiJogosAsync(2))
            .ReturnsAsync(false);

        // Act
        await _service.ExcluirAsync(2);

        // Assert
        _generoRepoMock.Verify(r => r.BuscarPorIdAsync(2), Times.Once);
        _generoRepoMock.Verify(r => r.PossuiJogosAsync(2), Times.Once);
        _generoRepoMock.Verify(r => r.ExcluirAsync(2), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_GeneroInexistente_LancaGeneroNaoEncontrado()
    {
        // Arrange
        _generoRepoMock
            .Setup(r => r.BuscarPorIdAsync(99))
            .ReturnsAsync((Genero?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.ExcluirAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNaoEncontrado, excecao.Erro);
        _generoRepoMock.Verify(r => r.PossuiJogosAsync(It.IsAny<int>()), Times.Never);
        _generoRepoMock.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
