using Moq;
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
}
