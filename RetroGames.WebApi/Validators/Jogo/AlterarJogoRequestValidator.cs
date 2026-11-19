using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class AlterarJogoRequestValidator : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Descricao).MaximumLength(1000);
        RuleFor(x => x.AnoLancamento).InclusiveBetween(1970, 2100);
        RuleFor(x => x.Preco).GreaterThan(0).Must(p => decimal.Round(p, 2) == p)
            .WithMessage("O preço deve ter até duas casas decimais.");
        RuleFor(x => x.Estoque).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IdPlataforma).GreaterThan(0);
        RuleFor(x => x.IdCondicaoJogo).GreaterThan(0);
        RuleFor(x => x.IdsGeneros).NotEmpty();
    }
}
