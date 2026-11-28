using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _service;

    public PedidoController(IPedidoService service)
    {
        _service = service;
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>>> Listar(
        [FromQuery] int? idSituacao = null)
    {
        var pedidos = await _service.ListarTodosAsync(idSituacao);
        return Ok(new RetroGamesResponseApiModel<List<ListarPedidoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = pedidos.Select(p => p.ParaListarResponse()).ToList()
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<InserirPedidoResponseApiModel>>> Inserir(
        [FromBody] InserirPedidoRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaItensSolicitados());
        var envelope = new RetroGamesResponseApiModel<InserirPedidoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criado.ParaInserirResponse()
        };
        return Created($"/api/Pedido/{criado.Pedido.Id}", envelope);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}/situacao")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> AlterarSituacao(
        int id,
        [FromBody] AlterarSituacaoPedidoRequestApiModel request)
    {
        await _service.AlterarSituacaoAsync(id, request.IdSituacaoPedido);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }
}
