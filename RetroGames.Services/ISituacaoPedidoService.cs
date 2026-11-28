using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface ISituacaoPedidoService
{
    Task<List<SituacaoPedido>> ListarAsync();
}
