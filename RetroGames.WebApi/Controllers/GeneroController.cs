using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;
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

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarGeneroResponseApiModel>>> ListarPorId(int id)
    {
        var genero = await _service.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarGeneroResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = genero.ParaResponse()
        });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarGeneroResponseApiModel>>> Inserir(
        [FromBody] InserirGeneroRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaModel());
        var envelope = new RetroGamesResponseApiModel<ListarGeneroResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criado.ParaResponse()
        };
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Id }, envelope);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Alterar(int id,
        [FromBody] AlterarGeneroRequestApiModel request)
    {
        await _service.AlterarAsync(request.ParaModel(id));
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }
}
