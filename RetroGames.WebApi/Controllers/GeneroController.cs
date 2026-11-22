using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _service;

    public GeneroController(IGeneroService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var itens = await _service.ListarAsync();
        return Ok(new RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = itens.Select(item => item.ParaResponse()).ToList()
        });
    }
}
