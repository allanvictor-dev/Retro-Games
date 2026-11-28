namespace RetroGames.Services;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> InserirAsync(List<ItemPedidoSolicitadoServiceModel> itensSolicitados);
}
