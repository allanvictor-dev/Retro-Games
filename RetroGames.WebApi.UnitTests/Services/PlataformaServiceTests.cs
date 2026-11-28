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
}
