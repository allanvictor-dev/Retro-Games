using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Genero;

public class AlterarGeneroRequestValidator : AbstractValidator<AlterarGeneroRequestApiModel>
{
    public AlterarGeneroRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(80);
    }
}
