using System.Transactions;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IUsuarioAtualService _usuarioAtual;

    public PedidoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<PedidoDetalheServiceModel> InserirAsync(List<ItemPedidoSolicitadoServiceModel> itensSolicitados)
    {
        var idUsuario = _usuarioAtual.IdUsuario
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado na sessão.");

        var idsJogos = itensSolicitados.Select(i => i.IdJogo).Distinct().ToList();

        Pedido pedido;
        var itensDetalhados = new List<ItemPedidoDetalheServiceModel>();

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            var jogos = await _repositorio.Jogo.ListarPorIdsAsync(idsJogos);
            var jogosPorId = jogos.ToDictionary(j => j.Id);

            decimal valorTotal = 0;

            foreach (var itemSolicitado in itensSolicitados)
            {
                if (!jogosPorId.TryGetValue(itemSolicitado.IdJogo, out var jogo) || !jogo.Ativo)
                    throw new BusinessRuleException(BusinessErrors.JogoIndisponivel);

                if (jogo.Estoque < itemSolicitado.Quantidade)
                    throw new BusinessRuleException(BusinessErrors.EstoqueInsuficiente);

                valorTotal += jogo.Preco * itemSolicitado.Quantidade;
            }

            pedido = new Pedido
            {
                Data = DateTime.UtcNow,
                ValorTotal = valorTotal,
                IdUsuario = idUsuario,
                IdSituacaoPedido = SituacaoPedidoIds.Pendente
            };

            pedido.Id = await _repositorio.Pedido.InserirAsync(pedido);

            foreach (var itemSolicitado in itensSolicitados)
            {
                var jogo = jogosPorId[itemSolicitado.IdJogo];
                var itemPedido = new ItemPedido
                {
                    IdPedido = pedido.Id,
                    IdJogo = jogo.Id,
                    Quantidade = itemSolicitado.Quantidade,
                    ValorUnitario = jogo.Preco
                };

                itemPedido.Id = await _repositorio.ItemPedido.InserirAsync(itemPedido);
                await _repositorio.Jogo.BaixarEstoqueAsync(jogo.Id, itemSolicitado.Quantidade);

                itensDetalhados.Add(new ItemPedidoDetalheServiceModel
                {
                    Item = itemPedido,
                    NomeJogo = jogo.Nome
                });
            }

            transactionScope.Complete();
        }

        var situacao = await _repositorio.SituacaoPedido.BuscarPorIdAsync(pedido.IdSituacaoPedido);

        return new PedidoDetalheServiceModel
        {
            Pedido = pedido,
            DescricaoSituacao = situacao?.Descricao ?? string.Empty,
            Itens = itensDetalhados
        };
    }
}
