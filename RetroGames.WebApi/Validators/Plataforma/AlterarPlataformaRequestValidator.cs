using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Plataforma;

public class AlterarPlataformaRequestValidator : AbstractValidator<AlterarPlataformaRequestApiModel>
{
    public AlterarPlataformaRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Fabricante).NotEmpty().MaximumLength(80);
        RuleFor(x => x.AnoLancamento).InclusiveBetween(1970, 2100);
    }
}
