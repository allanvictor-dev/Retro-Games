using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPedidoRepository
{
    Task<int> InserirAsync(Pedido pedido);

    Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario);

    Task<List<Pedido>> ListarTodosAsync(int? idSituacaoPedido = null);

    Task<Pedido?> BuscarPorIdAsync(int id);

    Task AtualizarSituacaoAsync(int id, int idSituacaoPedido);
}
