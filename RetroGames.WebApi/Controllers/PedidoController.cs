using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
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
}
