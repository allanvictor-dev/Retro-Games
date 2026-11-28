using System.Net;
using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class AutenticacaoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorySetMock;
    private readonly Mock<IPasswordHasher<Usuario>> _passwordHasherMock;
    private readonly AutenticacaoService _service;

    public AutenticacaoServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _repositorySetMock = new Mock<IRepositorySet>();
        _repositorySetMock.Setup(r => r.Usuario).Returns(_usuarioRepositoryMock.Object);
        _passwordHasherMock = new Mock<IPasswordHasher<Usuario>>();
        _service = new AutenticacaoService(_repositorySetMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task CadastrarAsync_DadosValidos_NormalizaGeraHashESalvaComoClienteAtivo()
    {
        Usuario? usuarioInserido = null;

        _usuarioRepositoryMock
            .Setup(r => r.ExisteComEmailAsync("maria@exemplo.com"))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<Usuario>(), "senha123"))
            .Returns("hash_seguro");

        _usuarioRepositoryMock
            .Setup(r => r.InserirAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => usuarioInserido = u)
            .ReturnsAsync(42);

        var resultado = await _service.CadastrarAsync("  Maria Silva  ", "  Maria@Exemplo.COM  ", "senha123");

        Assert.NotNull(usuarioInserido);
        Assert.Equal(42, resultado.Id);
        Assert.Equal("Maria Silva", resultado.Nome);
        Assert.Equal("maria@exemplo.com", resultado.Email);
        Assert.Equal("hash_seguro", resultado.SenhaHash);
        Assert.True(resultado.Ativo);
        Assert.Equal(PerfilIds.Cliente, resultado.IdPerfil);
        _usuarioRepositoryMock.Verify(r => r.ExisteComEmailAsync("maria@exemplo.com"), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.InserirAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarAsync_EmailJaExiste_LancaBusinessRuleExceptionEmailJaCadastrado()
    {
        _usuarioRepositoryMock
            .Setup(r => r.ExisteComEmailAsync("maria@exemplo.com"))
            .ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CadastrarAsync("Maria", "Maria@Exemplo.com", "senha123"));

        Assert.Equal(BusinessErrors.EmailJaCadastrado.Codigo, excecao.Erro.Codigo);
        Assert.Equal(HttpStatusCode.Conflict, excecao.Erro.Status);
        _usuarioRepositoryMock.Verify(r => r.InserirAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_RegistroDuplicadoNoBanco_LancaBusinessRuleExceptionEmailJaCadastrado()
    {
        _usuarioRepositoryMock
            .Setup(r => r.ExisteComEmailAsync("maria@exemplo.com"))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<Usuario>(), "senha123"))
            .Returns("hash_seguro");

        _usuarioRepositoryMock
            .Setup(r => r.InserirAsync(It.IsAny<Usuario>()))
            .ThrowsAsync(new RegistroDuplicadoException("ds_email duplicado"));

        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CadastrarAsync("Maria", "maria@exemplo.com", "senha123"));

        Assert.Equal(BusinessErrors.EmailJaCadastrado.Codigo, excecao.Erro.Codigo);
        Assert.Equal(HttpStatusCode.Conflict, excecao.Erro.Status);
    }
}
