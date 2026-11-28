using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private const int IdCliente = 8;
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPedidoRepository> _pedidos = new();
    private readonly Mock<IItemPedidoRepository> _itens = new();
    private readonly Mock<ISituacaoPedidoRepository> _situacoes = new();
    private readonly PedidoService _servico;

    public PedidoServiceTests()
    {
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        repositorios.SetupGet(r => r.Pedido).Returns(_pedidos.Object);
        repositorios.SetupGet(r => r.ItemPedido).Returns(_itens.Object);
        repositorios.SetupGet(r => r.SituacaoPedido).Returns(_situacoes.Object);
        var cliente = Mock.Of<IUsuarioAtualService>(u => u.IdUsuario == IdCliente);
        _servico = new PedidoService(repositorios.Object, cliente);
    }

    [Fact]
    public async Task InserirAsync_CompraValida_GravaTotalEmitePedidoEmUtcEBaixaEstoque()
    {
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<List<int>>())).ReturnsAsync([
            new() { Id = 4, Ativo = true, Estoque = 2, Preco = 12.50m },
            new() { Id = 5, Ativo = true, Estoque = 4, Preco = 7.75m }
        ]);
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>())).ReturnsAsync(20);

        var inicio = DateTime.UtcNow;
        var resultado = await _servico.InserirAsync([
            new() { IdJogo = 4, Quantidade = 2 }, new() { IdJogo = 5, Quantidade = 3 }
        ]);
        var fim = DateTime.UtcNow;

        Assert.Equal(20, resultado.Pedido.Id);
        Assert.Equal(48.25m, resultado.Pedido.ValorTotal);
        Assert.Equal(DateTimeKind.Utc, resultado.Pedido.Data.Kind);
        Assert.InRange(resultado.Pedido.Data, inicio, fim);
        _pedidos.Verify(r => r.InserirAsync(It.Is<Pedido>(p => p.IdUsuario == IdCliente &&
            p.ValorTotal == 48.25m && p.IdSituacaoPedido == SituacaoPedidoIds.Pendente &&
            p.Data == resultado.Pedido.Data)), Times.Once);
        _itens.Verify(r => r.InserirAsync(It.Is<ItemPedido>(i => i.IdPedido == 20 &&
            i.IdJogo == 4 && i.Quantidade == 2 && i.ValorUnitario == 12.50m)), Times.Once);
        _itens.Verify(r => r.InserirAsync(It.Is<ItemPedido>(i => i.IdPedido == 20 &&
            i.IdJogo == 5 && i.Quantidade == 3 && i.ValorUnitario == 7.75m)), Times.Once);
        _jogos.Verify(r => r.ListarPorIdsAsync(It.Is<List<int>>(ids =>
            ids.Order().SequenceEqual(new[] { 4, 5 }))), Times.Once);
        _jogos.Verify(r => r.BaixarEstoqueAsync(4, 2), Times.Once);
        _jogos.Verify(r => r.BaixarEstoqueAsync(5, 3), Times.Once);
        _itens.VerifyNoOtherCalls();
        _pedidos.VerifyNoOtherCalls();
        _jogos.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task InserirAsync_JogoInexistenteOuInativo_RejeitaCompra(bool jogoExiste)
    {
        List<Jogo> jogos = jogoExiste ? [new() { Id = 4, Ativo = false, Estoque = 5 }] : [];
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(jogos);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _servico.InserirAsync([new() { IdJogo = 4, Quantidade = 1 }]));

        Assert.Equal(BusinessErrors.JogoIndisponivel, erro.Erro);
        VerificarCompraSemGravacao();
    }

    [Fact]
    public async Task InserirAsync_SegundoJogoSemEstoque_RejeitaCompraInteira()
    {
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<List<int>>())).ReturnsAsync([
            new() { Id = 4, Ativo = true, Estoque = 3 },
            new() { Id = 5, Ativo = true, Estoque = 1 }
        ]);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _servico.InserirAsync([new() { IdJogo = 4, Quantidade = 1 }, new() { IdJogo = 5, Quantidade = 2 }]));

        Assert.Equal(BusinessErrors.EstoqueInsuficiente, erro.Erro);
        VerificarCompraSemGravacao();
    }

    private void VerificarCompraSemGravacao()
    {
        _pedidos.Verify(r => r.InserirAsync(It.IsAny<Pedido>()), Times.Never);
        _itens.Verify(r => r.InserirAsync(It.IsAny<ItemPedido>()), Times.Never);
        _jogos.Verify(r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AlterarSituacaoAsync_SituacaoInexistente_RejeitaAlteracao()
    {
        _pedidos.Setup(r => r.BuscarPorIdAsync(20)).ReturnsAsync(new Pedido { Id = 20 });
        _situacoes.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((SituacaoPedido?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.AlterarSituacaoAsync(20, 99));

        Assert.Equal(BusinessErrors.SituacaoNaoEncontrada, erro.Erro);
        _situacoes.Verify(r => r.BuscarPorIdAsync(99), Times.Once);
        _pedidos.Verify(r => r.AtualizarSituacaoAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AlterarSituacaoAsync_SituacaoExistente_AtualizaPedido()
    {
        _pedidos.Setup(r => r.BuscarPorIdAsync(20)).ReturnsAsync(new Pedido { Id = 20 });
        _situacoes.Setup(r => r.BuscarPorIdAsync(SituacaoPedidoIds.Pago))
            .ReturnsAsync(new SituacaoPedido { Id = SituacaoPedidoIds.Pago });

        await _servico.AlterarSituacaoAsync(20, SituacaoPedidoIds.Pago);

        _pedidos.Verify(r => r.BuscarPorIdAsync(20), Times.Once);
        _situacoes.Verify(r => r.BuscarPorIdAsync(SituacaoPedidoIds.Pago), Times.Once);
        _pedidos.Verify(r => r.AtualizarSituacaoAsync(20, SituacaoPedidoIds.Pago), Times.Once);
        _pedidos.VerifyNoOtherCalls();
    }
}
