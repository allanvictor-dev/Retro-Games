namespace RetroGames.Services;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> InserirAsync(List<ItemPedidoSolicitadoServiceModel> itensSolicitados);

    Task<List<PedidoDetalheServiceModel>> ListarTodosAsync(int? idSituacaoPedido = null);

    Task<List<PedidoDetalheServiceModel>> ListarMeusAsync();

    Task<PedidoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task AlterarSituacaoAsync(int idPedido, int idSituacaoPedido);
}
