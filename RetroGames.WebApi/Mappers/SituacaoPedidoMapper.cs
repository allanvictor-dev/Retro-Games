using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class SituacaoPedidoMapper
{
    public static ListarSituacaoPedidoResponseApiModel ParaResponse(this SituacaoPedido entidade)
    {
        return new ListarSituacaoPedidoResponseApiModel
        {
            Id = entidade.Id,
            Descricao = entidade.Descricao
        };
    }
}
