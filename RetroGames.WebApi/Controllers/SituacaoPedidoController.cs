using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SituacaoPedidoController : ControllerBase
{
    private readonly ISituacaoPedidoService _service;

    public SituacaoPedidoController(ISituacaoPedidoService service)
    {
        _service = service;
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarSituacaoPedidoResponseApiModel>>>> Listar()
    {
        var itens = await _service.ListarAsync();
        return Ok(new RetroGamesResponseApiModel<List<ListarSituacaoPedidoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = itens.Select(item => item.ParaResponse()).ToList()
        });
    }
}
