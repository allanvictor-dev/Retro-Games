namespace RetroGames.Services;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> InserirAsync(List<ItemPedidoSolicitadoServiceModel> itensSolicitados);

    Task<List<PedidoDetalheServiceModel>> ListarTodosAsync(int? idSituacaoPedido = null);

    Task AlterarSituacaoAsync(int idPedido, int idSituacaoPedido);
}
