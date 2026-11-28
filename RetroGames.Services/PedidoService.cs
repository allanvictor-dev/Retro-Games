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

    public async Task<List<PedidoDetalheServiceModel>> ListarTodosAsync(int? idSituacaoPedido = null)
    {
        var pedidos = await _repositorio.Pedido.ListarTodosAsync(idSituacaoPedido);
        return await MontarDetalhesAsync(pedidos);
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarMeusAsync()
    {
        var idUsuario = _usuarioAtual.IdUsuario
            ?? throw new InvalidOperationException("Usuário autenticado não encontrado na sessão.");
        var pedidos = await _repositorio.Pedido.ListarPorUsuarioAsync(idUsuario);
        return await MontarDetalhesAsync(pedidos);
    }

    public async Task<PedidoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var pedido = await _repositorio.Pedido.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.PedidoNaoEncontrado);

        if (pedido.IdUsuario != _usuarioAtual.IdUsuario && !_usuarioAtual.EhAdministrador)
            throw new BusinessRuleException(BusinessErrors.PedidoDeOutroUsuario);

        var detalhes = await MontarDetalhesAsync([pedido]);
        return detalhes.Single();
    }

    public async Task AlterarSituacaoAsync(int idPedido, int idSituacaoPedido)
    {
        var pedido = await _repositorio.Pedido.BuscarPorIdAsync(idPedido);
        if (pedido == null)
            throw new BusinessRuleException(BusinessErrors.PedidoNaoEncontrado);

        var situacao = await _repositorio.SituacaoPedido.BuscarPorIdAsync(idSituacaoPedido);
        if (situacao == null)
            throw new BusinessRuleException(BusinessErrors.SituacaoNaoEncontrada);

        await _repositorio.Pedido.AtualizarSituacaoAsync(idPedido, idSituacaoPedido);
    }

    private async Task<List<PedidoDetalheServiceModel>> MontarDetalhesAsync(List<Pedido> pedidos)
    {
        if (pedidos.Count == 0)
            return new List<PedidoDetalheServiceModel>();

        var situacoes = await _repositorio.SituacaoPedido.ListarAsync();
        var usuarios = await _repositorio.Usuario.ListarPorIdsAsync(
            pedidos.Select(p => p.IdUsuario).Distinct().ToList());
        var itens = await _repositorio.ItemPedido.ListarPorPedidosAsync(
            pedidos.Select(p => p.Id).ToList());
        var jogos = await _repositorio.Jogo.ListarPorIdsAsync(
            itens.Select(i => i.IdJogo).Distinct().ToList());

        var situacoesPorId = situacoes.ToDictionary(s => s.Id);
        var usuariosPorId = usuarios.ToDictionary(u => u.Id);
        var jogosPorId = jogos.ToDictionary(j => j.Id);

        return pedidos.Select(pedido =>
        {
            usuariosPorId.TryGetValue(pedido.IdUsuario, out var usuario);
            situacoesPorId.TryGetValue(pedido.IdSituacaoPedido, out var situacao);

            var itensPedido = (from item in itens
                               where item.IdPedido == pedido.Id
                               select new ItemPedidoDetalheServiceModel
                               {
                                   Item = item,
                                   NomeJogo = jogosPorId.TryGetValue(item.IdJogo, out var jogo)
                                       ? jogo.Nome
                                       : string.Empty
                               }).ToList();

            return new PedidoDetalheServiceModel
            {
                Pedido = pedido,
                DescricaoSituacao = situacao?.Descricao ?? string.Empty,
                NomeUsuario = usuario?.Nome ?? string.Empty,
                EmailUsuario = usuario?.Email ?? string.Empty,
                Itens = itensPedido
            };
        }).ToList();
    }
}
