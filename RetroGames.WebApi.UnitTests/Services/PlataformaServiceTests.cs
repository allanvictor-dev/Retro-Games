using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorioMock;
    private readonly PlataformaService _servico;

    public PlataformaServiceTests()
    {
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _repositorioMock = new Mock<IRepositorySet>();

        _repositorioMock
            .Setup(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);

        _servico = new PlataformaService(_repositorioMock.Object);
    }

    [Fact]
    public async Task BuscarPorIdAsync_PlataformaInexistente_LancaBusinessRuleException()
    {
        // Arrange
        const int idInexistente = 999;

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Plataforma?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.BuscarPorIdAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNaoEncontrada, excecao.Erro);
        _plataformaRepositoryMock.Verify(r => r.BuscarPorIdAsync(idInexistente), Times.Once);
    }

    [Fact]
    public async Task InserirAsync_NomeDuplicado_LancaBusinessRuleException()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "Super Nintendo" };

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("Super Nintendo", null))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, excecao.Erro);
        _plataformaRepositoryMock.Verify(r => r.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Fact]
    public async Task InserirAsync_RegistroDuplicadoNoBanco_LancaBusinessRuleException()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "Super Nintendo" };

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("Super Nintendo", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException("Registro duplicado."));

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, excecao.Erro);
        _plataformaRepositoryMock.Verify(r => r.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task InserirAsync_NomeDisponivel_RetornaPlataformaComIdGerado()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "  Mega Drive  " };

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("Mega Drive", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ReturnsAsync(10);

        // Act
        var resultado = await _servico.InserirAsync(plataforma);

        // Assert
        Assert.Equal(10, resultado.Id);
        Assert.Equal("Mega Drive", resultado.Nome);
        _plataformaRepositoryMock.Verify(r => r.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_MantendoProprioNome_AlteraComSucesso()
    {
        // Arrange
        var plataformaExistente = new Plataforma { Id = 1, Nome = "PlayStation" };
        var plataformaAlterada = new Plataforma { Id = 1, Nome = "PlayStation" };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(plataformaExistente);

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("PlayStation", 1))
            .ReturnsAsync(false);

        // Act
        await _servico.AlterarAsync(plataformaAlterada);

        // Assert
        _plataformaRepositoryMock.Verify(r => r.ExisteComNomeAsync("PlayStation", 1), Times.Once);
        _plataformaRepositoryMock.Verify(r => r.AlterarAsync(plataformaAlterada), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_PlataformaAssociadaAJogos_LancaBusinessRuleException()
    {
        // Arrange
        const int idPlataforma = 1;
        var plataforma = new Plataforma { Id = idPlataforma, Nome = "Nintendo 64" };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(idPlataforma))
            .ReturnsAsync(plataforma);

        _plataformaRepositoryMock
            .Setup(r => r.PossuiJogosAsync(idPlataforma))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _servico.ExcluirAsync(idPlataforma));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaPossuiJogos, excecao.Erro);
        _plataformaRepositoryMock.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
