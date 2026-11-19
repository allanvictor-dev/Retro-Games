using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class ListarJogoRequestValidator : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(x => x.Pagina).GreaterThan(0);
        RuleFor(x => x.TamanhoPagina).InclusiveBetween(1, 50);
    }
}
