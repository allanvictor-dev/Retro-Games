using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class SituacaoPedidoService : ISituacaoPedidoService
{
    private readonly IRepositorySet _repositorio;

    public SituacaoPedidoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        return await _repositorio.SituacaoPedido.ListarAsync();
    }
}
