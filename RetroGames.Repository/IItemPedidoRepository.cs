using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IItemPedidoRepository
{
    Task<int> InserirAsync(ItemPedido item);

    Task<List<ItemPedido>> ListarPorPedidoAsync(int idPedido);

    Task<List<ItemPedido>> ListarPorPedidosAsync(List<int> idsPedidos);
}
