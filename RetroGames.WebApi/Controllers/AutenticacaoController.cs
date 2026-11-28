using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _service;

    public AutenticacaoController(IAutenticacaoService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        [FromBody] LoginRequestApiModel request)
    {
        var usuario = await _service.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null)
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage
            });

        var principal = usuario.ParaClaimsPrincipal();
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Ok(MontarResposta(principal));
    }

    [AllowAnonymous]
    [HttpPost("cadastrar")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Cadastrar(
        [FromBody] CadastrarUsuarioRequestApiModel request)
    {
        var usuario = await _service.CadastrarAsync(request.Nome, request.Email, request.Senha);
        var principal = usuario.ParaClaimsPrincipal();
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return CreatedAtAction(nameof(Eu), MontarResposta(principal));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }

    [Authorize]
    [HttpGet("eu")]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        return Ok(MontarResposta(User));
    }

    private static RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel> MontarResposta(
        ClaimsPrincipal usuario)
    {
        return new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = new UsuarioLogadoResponseApiModel
            {
                Id = usuario.ObterId() ?? throw new InvalidOperationException("Usuário sem identificador."),
                Nome = usuario.Identity?.Name ?? string.Empty,
                Email = usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                Perfil = usuario.FindFirstValue(ClaimTypes.Role) ?? string.Empty
            }
        };
    }
}
