using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();
        var (jogos, total) = await _service.ListarAsync(
            filtro.Pagina, filtro.TamanhoPagina, filtro.Nome, filtro.IdPlataforma, filtro.IdGenero,
            filtro.IncluirInativos);

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = new PaginaResponseApiModel<ListarJogoResponseApiModel>
            {
                Itens = jogos.Select(j => j.ParaResponse()).ToList(),
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalItens = total,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.TamanhoPagina)
            }
        });
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var jogo = await _service.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogo.ParaDetalheResponse()
        });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        [FromBody] InserirJogoRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaModel(), request.IdsGeneros);
        var detalhe = await _service.BuscarPorIdAsync(criado.Id);
        var envelope = new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaDetalheResponse()
        };
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Id }, envelope);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Alterar(int id,
        [FromBody] AlterarJogoRequestApiModel request)
    {
        await _service.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await _service.DesativarAsync(id);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }
}
