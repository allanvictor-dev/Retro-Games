using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class ConsultaPedidoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorios = new() { DefaultValue = DefaultValue.Mock };
    private readonly Mock<IUsuarioAtualService> _usuario = new();
    private readonly PedidoService _servico;

    public ConsultaPedidoServiceTests()
    {
        _usuario.SetupGet(u => u.IdUsuario).Returns(7);
        _servico = new PedidoService(_repositorios.Object, _usuario.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task BuscarPorIdAsync_InexistenteOuAlheio_RejeitaAntesDeCarregarItens(bool existe)
    {
        _repositorios.Setup(r => r.Pedido.BuscarPorIdAsync(10))
            .ReturnsAsync(existe ? new Pedido { Id = 10, IdUsuario = 9 } : null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.BuscarPorIdAsync(10));

        Assert.Equal(existe ? BusinessErrors.PedidoDeOutroUsuario : BusinessErrors.PedidoNaoEncontrado, erro.Erro);
        _repositorios.Verify(r => r.Pedido.BuscarPorIdAsync(10), Times.Once);
        _repositorios.VerifyGet(r => r.ItemPedido, Times.Never);
        _repositorios.VerifyGet(r => r.Usuario, Times.Never);
    }

    [Theory]
    [InlineData(7, false)]
    [InlineData(9, true)]
    public async Task BuscarPorIdAsync_DonoOuAdministrador_RecebePedido(int idDono, bool administrador)
    {
        _usuario.SetupGet(u => u.EhAdministrador).Returns(administrador);
        _repositorios.Setup(r => r.Pedido.BuscarPorIdAsync(10))
            .ReturnsAsync(new Pedido { Id = 10, IdUsuario = idDono, IdSituacaoPedido = 1 });
        ConfigurarDetalhes();

        var detalhe = await _servico.BuscarPorIdAsync(10);

        Assert.Equal(10, detalhe.Pedido.Id);
        Assert.Equal(idDono, detalhe.Pedido.IdUsuario);
        Assert.Equal("Sonic", Assert.Single(detalhe.Itens).NomeJogo);
    }

    [Fact]
    public async Task ListarMeusAsync_ConsultaUsuarioDaSessaoEPreservaOrdemEPrecoPago()
    {
        var hoje = new DateTime(2026, 1, 2);
        _repositorios.Setup(r => r.Pedido.ListarPorUsuarioAsync(7)).ReturnsAsync([
            new() { Id = 10, IdUsuario = 7, Data = hoje, ValorTotal = 29m, IdSituacaoPedido = 1 },
            new() { Id = 8, IdUsuario = 7, Data = hoje.AddDays(-1), IdSituacaoPedido = 1 }
        ]);
        ConfigurarDetalhes();

        var pedidos = await _servico.ListarMeusAsync();

        _repositorios.Verify(r => r.Pedido.ListarPorUsuarioAsync(7), Times.Once);
        _repositorios.Verify(r => r.Pedido.ListarTodosAsync(It.IsAny<int?>()), Times.Never);
        Assert.Equal(new[] { 10, 8 }, pedidos.Select(p => p.Pedido.Id));
        Assert.Empty(pedidos[1].Itens);
        Assert.Equal("Pendente", pedidos[0].DescricaoSituacao);
        Assert.Equal(29m, pedidos[0].Pedido.ValorTotal);
        var detalhe = Assert.Single(pedidos[0].Itens);
        Assert.Equal("Sonic", detalhe.NomeJogo);
        Assert.Equal(2, detalhe.Item.Quantidade);
        Assert.Equal(14.50m, detalhe.Item.ValorUnitario);
    }

    private void ConfigurarDetalhes()
    {
        _repositorios.Setup(r => r.SituacaoPedido.ListarAsync())
            .ReturnsAsync([new() { Id = 1, Descricao = "Pendente" }]);
        _repositorios.Setup(r => r.Usuario.ListarPorIdsAsync(It.IsAny<List<int>>())).ReturnsAsync([]);
        _repositorios.Setup(r => r.ItemPedido.ListarPorPedidosAsync(It.IsAny<List<int>>()))
            .ReturnsAsync([new() { IdPedido = 10, IdJogo = 3, Quantidade = 2, ValorUnitario = 14.50m }]);
        _repositorios.Setup(r => r.Jogo.ListarPorIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync([new() { Id = 3, Nome = "Sonic", Preco = 99m }]);
    }
}
