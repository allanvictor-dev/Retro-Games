using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PedidoMapper
{
    public static List<ItemPedidoSolicitadoServiceModel> ParaItensSolicitados(
        this InserirPedidoRequestApiModel request)
    {
        return request.Itens.Select(item => new ItemPedidoSolicitadoServiceModel
        {
            IdJogo = item.IdJogo,
            Quantidade = item.Quantidade
        }).ToList();
    }

    public static ItemPedidoResponseApiModel ParaResponse(this ItemPedidoDetalheServiceModel detalhe)
    {
        var item = detalhe.Item;
        return new ItemPedidoResponseApiModel
        {
            IdJogo = item.IdJogo,
            NomeJogo = detalhe.NomeJogo,
            Quantidade = item.Quantidade,
            ValorUnitario = item.ValorUnitario,
            Subtotal = item.ValorUnitario * item.Quantidade
        };
    }

    public static InserirPedidoResponseApiModel ParaInserirResponse(this PedidoDetalheServiceModel detalhe)
    {
        var pedido = detalhe.Pedido;
        return new InserirPedidoResponseApiModel
        {
            Id = pedido.Id,
            Data = pedido.Data,
            ValorTotal = pedido.ValorTotal,
            IdSituacaoPedido = pedido.IdSituacaoPedido,
            DescricaoSituacao = detalhe.DescricaoSituacao,
            Itens = detalhe.Itens.Select(i => i.ParaResponse()).ToList()
        };
    }
}
